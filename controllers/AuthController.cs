using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using api.Data;
using dotnet_cSharp.DTOs;
using dotnet_cSharp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_cSharp.Controller
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly NereyeDBContext _dapper;
        private readonly AuthHelper _authHelper;

        public AuthController(IConfiguration config)
        {
            _dapper = new NereyeDBContext(config);
            _authHelper = new AuthHelper(config);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto userForRegistrationDto)
        {
            if (userForRegistrationDto.Password == userForRegistrationDto.PasswordConfirm)
            {
                string sqlCheckUserExists =
                    "SELECT Email FROM dbo.Auth WHERE Email = '"
                    + userForRegistrationDto.Email
                    + "'";
                IEnumerable<string> existingUser = _dapper.LoadData<string>(sqlCheckUserExists);
                if (existingUser.Count() == 0)
                {
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = _authHelper.GetPasswordHash(
                        userForRegistrationDto.Password,
                        passwordSalt
                    );
                    string sqlAddAuth =
                        @"
                        INSERT INTO dbo.Auth  (Email,
                        PasswordHash,
                        PasswordSalt) VALUES ('"
                        + userForRegistrationDto.Email
                        + "', @PasswordHash, @PasswordSalt)";
                    List<SqlParameter> sqlParameters = new List<SqlParameter>();

                    SqlParameter passwordSaltParameter = new SqlParameter(
                        "@PasswordSalt",
                        SqlDbType.VarBinary
                    );
                    passwordSaltParameter.Value = passwordSalt;

                    SqlParameter passwordHashParameter = new SqlParameter(
                        "@PasswordHash",
                        SqlDbType.VarBinary
                    );
                    passwordHashParameter.Value = passwordHash;

                    sqlParameters.Add(passwordSaltParameter);
                    sqlParameters.Add(passwordHashParameter);
                    if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
                    {
                        string sqlAddUser =
                            @"
                            INSERT INTO dbo.Users ([FirstName],[LastName],[Email],[Active]) 
                            VALUES (@FirstName,@LastName,@Email,@Active)";

                        // Define parameters to pass to Dapper
                        var parameters = new
                        {
                            FirstName = userForRegistrationDto.FirstName,
                            LastName = userForRegistrationDto.LastName,
                            Email = userForRegistrationDto.Email,
                            Active = 1,
                        };
                        bool result = _dapper.ExecuteSql(sqlAddUser, parameters);
                        if (result)
                        {
                            return Ok("user registered and added to the table");
                        }
                        throw new Exception("Failed to add user.");
                    }
                    throw new Exception("failed to register user");
                }
                throw new Exception("user with this email already exists");
            }
            throw new Exception("passwords do not match");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLoginDto)
        {
            string sqlForHashAndSalt =
                @"SELECT 
                [PasswordHash],
                [PasswordSalt] FROM dbo.Auth WHERE Email = '"
                + userForLoginDto.Email
                + "'";

            UserForLoginConfirmationDto userForConfirmationDto =
                _dapper.LoadDataSingleAuth<UserForLoginConfirmationDto>(sqlForHashAndSalt);
            byte[] passwordHash = _authHelper.GetPasswordHash(
                userForLoginDto.Password,
                userForConfirmationDto.PasswordSalt
            );

            for (int index = 0; index < passwordHash.Length; index++)
            {
                if (passwordHash[index] != userForConfirmationDto.PasswordHash[index])
                {
                    return StatusCode(401, "Incorrect password!");
                }
            }
            string userIdSql =
                @"SELECT UserId FROM dbo.Users WHERE Email = '" + userForLoginDto.Email + "'";
            int userId = _dapper.LoadDataSingleAuth<int>(userIdSql);

            return Ok(
                new Dictionary<string, string> { { "token", _authHelper.CreateToken(userId) } }
            );
        }

        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            string userId = User.FindFirst("userId")?.Value + ""; // User is from controllerbase
            string userIdSql = @"SELECT UserId FROM dbo.Users WHERE UserId =" + userId;
            int userIdFromDb = _dapper.LoadDataSingleAuth<int>(userIdSql);

            return Ok(
                new Dictionary<string, string>
                {
                    { "token", _authHelper.CreateToken(userIdFromDb) },
                }
            );
        }

        [Authorize]
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            // No specific action is required on the server side for logout if stateless tokens are used.
            // The client simply needs to remove the token from local storage/session.
            return Ok("User logged out successfully.");
        }
    }
}
