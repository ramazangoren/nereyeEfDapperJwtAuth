using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.UserDto;
using api.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace api.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IDbConnection _dbConnection;

        public UserController(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var sql = @"SELECT * FROM Users";
            var users = await _dbConnection.QueryAsync<User>(sql);
            return Ok(users);
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUser(int Id)
        {
            var sql = @"SELECT * FROM Users WHERE UserId =" + Id;
            var user = await _dbConnection.QueryAsync<User>(sql);
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<CreateUserDto>> CreateUser(CreateUserDto userDto)
        {
            if (
                userDto == null
                || string.IsNullOrWhiteSpace(userDto.UserName)
                || string.IsNullOrWhiteSpace(userDto.LastName)
                || string.IsNullOrWhiteSpace(userDto.Email)
                || string.IsNullOrWhiteSpace(userDto.UserPassword)
            )
            {
                return BadRequest("All fields are required.");
            }

            // if (userDto.UserPassword.Length < 8)
            // {
            //     return BadRequest("Password must be at least 8 characters long.");
            // }

            userDto.UserPassword = HashPassword(userDto.UserPassword);

            var query =
                @"INSERT INTO Users (UserName, LastName, Email, UserPassword, Avatar)
                  VALUES (@UserName, @LastName, @Email, @UserPassword, @Avatar);
                  SELECT CAST(SCOPE_IDENTITY() as int);";
                
                await _dbConnection.ExecuteAsync(query, userDto);
                return Ok(userDto);

            // try
            // {
            //     await _dbConnection.ExecuteAsync(query, userDto);
            //     return Ok(userDto);
            // }
            // catch
            // {
            //     return StatusCode(500, "An error occurred while creating the user.");
            // }
        }

        [HttpPut("{UserId}")]
        public async Task<ActionResult<UpdateUserDto>> UpdateUser(int UserId, UpdateUserDto userDto)
        {
            if (userDto == null)
                return BadRequest("User data is required.");

            var existingUser = await _dbConnection.QuerySingleOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE UserId = @UserId",
                new { UserId }
            );

            if (existingUser == null)
                return NotFound($"User with ID {UserId} not found.");

            if (!string.IsNullOrWhiteSpace(userDto.UserPassword))
            {
                userDto.UserPassword = HashPassword(userDto.UserPassword);
            }

            var query =
                @"UPDATE Users 
                    SET 
                        UserName = @UserName, 
                        LastName = @LastName, 
                        Email = @Email, 
                        UserPassword = @UserPassword, 
                        Avatar = @Avatar, 
                        UpdatedAt = @UpdatedAt
                    WHERE UserId = @UserId";

            await _dbConnection.ExecuteAsync(
                query,
                new
                {
                    userDto.UserName,
                    userDto.LastName,
                    userDto.Email,
                    userDto.UserPassword,
                    userDto.Avatar,
                    UpdatedAt = DateTime.Now,
                    UserId,
                }
            );

            var RetrievedUserData = "SELECT * FROM Users WHERE UserId = @UserId";
            var updatedUser = await _dbConnection.QuerySingleAsync<UpdateUserDto>(
                RetrievedUserData,
                new { UserId }
            );

            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            // Attempt to delete the user directly; if not found, return NotFound
            var rowsAffected = await _dbConnection.ExecuteAsync(
                "DELETE FROM Users WHERE UserId = @UserId",
                new { UserId = id }
            );

            return rowsAffected > 0 ? NoContent() : NotFound(); // Return 204 No Content or 404 Not Found
        }

        [HttpGet("MyRestaurants/{id}")]
        public async Task<ActionResult<IEnumerable<Restaurant>>> GetMyRestaurants(int id)
        {
            var sql = "SELECT * FROM Restaurants WHERE UserId = @UserId";
            var restaurants = await _dbConnection.QueryAsync<Restaurant>(sql, new { UserId = id });
            return Ok(restaurants);
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
