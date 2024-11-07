using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace api.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private readonly IDbConnection _dbConnection;

        public FavoriteController(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        [HttpGet("Favorites/{UserId:int}")]
        public async Task<ActionResult<IEnumerable<object>>> GetFavorites(int UserId)
        {
            var favoritesWithRestaurantInfo = await _dbConnection.QueryAsync(
                @"
                SELECT f.FavoriteId, f.RestaurantId, f.UserId, f.CreatedAt, r.RestaurantName, r.RestaurantCode
                FROM Favorites f
                JOIN Restaurants r ON f.RestaurantId = r.RestaurantId
                WHERE f.UserId = @UserId;",
                new { UserId }
            );

            return favoritesWithRestaurantInfo.Any()
                ? Ok(favoritesWithRestaurantInfo)
                : NotFound($"No favorites found for UserId: {UserId}");
        }

        // public async Task<ActionResult<AddFavoriteDto>> AddFavorite(int RestaurantId, int UserId)
        // i could've used this above method for below function but i didnt use it because i have nothing in AddFavoriteDto
        // im gon go delete it
        [HttpPost("AddFavorite/{RestaurantId:int}/{UserId:int}")]
        public async Task<ActionResult<string>> AddFavorite(int RestaurantId, int UserId)
        {
            // Check if the restaurant exists
            var restaurant = await _dbConnection.QueryFirstOrDefaultAsync<Restaurant>(
                "SELECT RestaurantId FROM Restaurants WHERE RestaurantId = @RestaurantId",
                new { RestaurantId }
            );

            if (restaurant == null)
                return NotFound($"Restaurant with ID {RestaurantId} not found.");

            // Prepare and execute the insertion query
            var favoriteQuery =
                @"
        INSERT INTO Favorites (RestaurantId, UserId, CreatedAt) 
        VALUES (@RestaurantId, @UserId, @CreatedAt);";

            var favoriteParameters = new
            {
                RestaurantId,
                UserId,
                CreatedAt = DateTime.Now,
            };

            await _dbConnection.ExecuteAsync(favoriteQuery, favoriteParameters);

            return CreatedAtAction(
                nameof(AddFavorite),
                new { RestaurantId, UserId },
                "Added to favorites"
            );
        }

        [HttpDelete("DeleteFavorite/{UserId:int}/{FavoriteId:int}")]
        public async Task<ActionResult<string>> DeleteFavorite(int FavoriteId, int UserId)
        {
            var sql =
                "SELECT COUNT(1) FROM Favorites WHERE FavoriteId = @FavoriteId AND UserId = @UserId";
            var favoriteExists = await _dbConnection.QueryFirstOrDefaultAsync<int>(
                sql,
                new { FavoriteId, UserId }
            );

            if (favoriteExists == 0)
                return NotFound($"Favorite with ID {FavoriteId} for User ID {UserId} not found.");

            var favoriteQuery =
                @"DELETE FROM Favorites WHERE FavoriteId = @FavoriteId AND UserId = @UserId";
            await _dbConnection.ExecuteAsync(favoriteQuery, new { FavoriteId, UserId });

            return Ok("Deleted from favorites");
        }
    }
}
