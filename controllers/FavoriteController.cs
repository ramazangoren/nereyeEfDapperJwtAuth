using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.DTOs.ReviewsDto;
using api.Helpers;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api.controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class FavoriteController : ControllerBase
    {
        private readonly NereyeDBContext _dapper;
        private readonly ILogger<FavoriteController> _logger;

        public FavoriteController(IConfiguration config, ILogger<FavoriteController> logger)
        {
            _dapper = new NereyeDBContext(config);
            _logger = logger;
        }

        [HttpGet("Favorites")]
        public ActionResult<IEnumerable<object>> GetFavorites()
        {
            try
            {
                string UserId = User.FindFirst("userId")?.Value + "";
                string sql =
                    @"
                SELECT f.FavoriteId, f.RestaurantId, f.UserId, f.CreatedAt, r.RestaurantName, r.RestaurantCode
                FROM Favorites f
                JOIN Restaurants r ON f.RestaurantId = r.RestaurantId
                WHERE f.UserId = " + UserId;
                var favoritesWithRestaurantInfo = _dapper.LoadData<object>(sql);

                return favoritesWithRestaurantInfo.Any()
                    ? Ok(favoritesWithRestaurantInfo)
                    : NotFound($"No favorites found for UserId: {UserId}");
            }
            catch (Exception ex)
            {
                return ErrorHandler.HandleError(ex, "GetFavorites", _logger);
            }
        }

        [HttpPost("AddFavorite/{RestaurantId:int}")]
        public ActionResult<string> AddFavorite(int RestaurantId)
        {
            try
            {
                string UserId = User.FindFirst("userId")?.Value + "";
                var restaurantExistsQuery =
                    "SELECT COUNT(1) FROM Restaurants WHERE RestaurantId = @RestaurantId";
                int restaurantExists = _dapper.ExecuteSqlWithRowCount(
                    restaurantExistsQuery,
                    new { RestaurantId }
                );

                if (restaurantExists == 0)
                {
                    return NotFound($"Restaurant with ID {RestaurantId} not found.");
                }
                var favoriteQuery =
                    @"INSERT INTO Favorites (RestaurantId, UserId, CreatedAt) 
                                VALUES (@RestaurantId, @UserId, @CreatedAt);";

                var favoriteParameters = new
                {
                    RestaurantId,
                    UserId,
                    CreatedAt = DateTime.Now,
                };

                bool result = _dapper.ExecuteSql(favoriteQuery, favoriteParameters);

                return result ? Ok("Added to favorites") : BadRequest("could add it to favorites");
            }
            catch (Exception ex)
            {
                return ErrorHandler.HandleError(ex, "AddFavorite", _logger);
            }
        }

        [HttpDelete("DeleteFavorite/{FavoriteId:int}")]
        public ActionResult<string> DeleteFavorite(int FavoriteId)
        {
            try
            {
                string UserId = User.FindFirst("userId")?.Value + "";
                var sql =
                    "SELECT COUNT(1) FROM Favorites WHERE FavoriteId = @FavoriteId AND UserId = @UserId";
                int favoriteExists = _dapper.ExecuteSqlWithRowCount(
                    sql,
                    new { FavoriteId, UserId }
                );

                if (favoriteExists == 0)
                    return NotFound(
                        $"Favorite with ID {FavoriteId} for User ID {UserId} not found."
                    );

                var favoriteQuery =
                    @"DELETE FROM Favorites WHERE FavoriteId = @FavoriteId AND UserId = @UserId";
                bool result = _dapper.ExecuteSql(favoriteQuery, new { FavoriteId, UserId });
                return result
                    ? Ok("Deleted from favorites")
                    : BadRequest("could delete it from favorites");
            }
            catch (Exception ex)
            {
                return ErrorHandler.HandleError(ex, "DeleteFavorite", _logger);
            }
        }
    }
}
