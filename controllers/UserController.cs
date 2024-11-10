using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Helpers;
using api.Models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        NereyeDBContext _dapper;
        private readonly ILogger<UserController> _logger;

        public UserController(IConfiguration config, ILogger<UserController> logger)
        {
            _dapper = new NereyeDBContext(config);
            _logger = logger;
        }

        [HttpGet("Profile")]
        public ActionResult<IEnumerable<object>> Profile()
        {
            try
            {
                string UserId = User.FindFirst("userId")?.Value + "";
                var myProfile = _dapper.LoadDataSingle<object>(
                    "SELECT * FROM dbo.Users WHERE UserId = @UserId",
                    new { UserId }
                );
                return Ok(myProfile);
            }
            catch (Exception ex)
            {
                return ErrorHandler.HandleError(ex, "Profile", _logger);
            }
        }

        [HttpGet("MyRestaurants")]
        public ActionResult<IEnumerable<Restaurant>> MyRestaurants()
        {
            try
            {
                string UserId = User.FindFirst("userId")?.Value + "";
                string sql = "SELECT * FROM dbo.Restaurants WHERE UserId = " + UserId;
                var myRestaurants = _dapper.LoadData<Restaurant>(sql);
                return Ok(myRestaurants);
            }
            catch (Exception ex)
            {
                // Log and handle the error using the ErrorHandler class
                return ErrorHandler.HandleError(ex, "MyRestaurants", _logger);
            }
        }
    }
}
