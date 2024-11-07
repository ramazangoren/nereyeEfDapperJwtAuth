using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.ReviewsDto;
using api.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace api.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IDbConnection _dbConnection;

        public ReviewsController(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        [HttpGet("RestaurantReviews/{RestaurantId:int}")]
        public async Task<ActionResult<Reviews>> GetReviews(int RestaurantId)
        {
            var reviews = await _dbConnection.QueryFirstOrDefaultAsync<Reviews>(
                "SELECT * FROM Reviews WHERE RestaurantId = @RestaurantId",
                new { RestaurantId }
            );
            return reviews == null ? NotFound("No reviews found") : Ok(reviews);
        }

        [HttpGet("GetSingleReview/{restaurantId:int}/{reviewId:int}")]
        public async Task<ActionResult<Reviews>> GetSingleReview(int restaurantId, int reviewId)
        {
            var singleReview = await _dbConnection.QueryFirstOrDefaultAsync<Reviews>(
                "SELECT * FROM Reviews WHERE ReviewId = @reviewId AND RestaurantId = @restaurantId",
                new { reviewId, restaurantId }
            );

            return singleReview == null ? NotFound("No reviews found.") : Ok(singleReview);
        }

        [HttpPost("AddReview/{RestaurantId:int}/{UserId:int}")]
        public async Task<ActionResult<AddReviewDto>> AddReview(
            AddReviewDto reviewDto,
            int RestaurantId,
            int UserId
        )
        {
            if (reviewDto == null)
                return BadRequest("Review data is required.");

            if (reviewDto.Rating < 1 || reviewDto.Rating > 5)
                return BadRequest("Rating must be between 1 and 5.");

            try
            {
                var restaurantExists = await _dbConnection.QueryFirstOrDefaultAsync<int>(
                    "SELECT COUNT(1) FROM Restaurants WHERE RestaurantId = @RestaurantId",
                    new { RestaurantId }
                );

                if (restaurantExists == 0)
                    return NotFound($"Restaurant with ID {RestaurantId} not found.");

                var query =
                    @"INSERT INTO Reviews (
                                RestaurantId, UserId, Comment, ReviewPhoto, Rating, CreatedAt
                            ) VALUES (
                                @RestaurantId, @UserId, @Comment, @ReviewPhoto, @Rating, @CreatedAt
                            );
                            SELECT CAST(SCOPE_IDENTITY() AS int);";

                var parameters = new
                {
                    RestaurantId,
                    UserId,
                    reviewDto.Comment,
                    reviewDto.ReviewPhoto,
                    reviewDto.Rating,
                    CreatedAt = DateTime.Now,
                };

                await _dbConnection.ExecuteAsync(query, parameters);

                return Ok(reviewDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("UpdateReview/{RestaurantId:int}/{UserId:int}/{ReviewId:int}")]
        public async Task<ActionResult<AddReviewDto>> UpdateReview(
            AddReviewDto reviewDto,
            int RestaurantId,
            int UserId,
            int ReviewId
        )
        {
            if (reviewDto == null)
                return BadRequest("Review data is required.");

            if (reviewDto.Rating < 1 || reviewDto.Rating > 5)
                return BadRequest("Rating must be between 1 and 5.");

            try
            {
                // Check if the restaurant exists
                var restaurantExists = await _dbConnection.QueryFirstOrDefaultAsync<int>(
                    "SELECT COUNT(1) FROM Restaurants WHERE RestaurantId = @RestaurantId",
                    new { RestaurantId }
                );

                if (restaurantExists == 0)
                    return NotFound($"Restaurant with ID {RestaurantId} not found.");

                // Check if the review exists and belongs to the user
                var userOwnTheReview = await _dbConnection.QueryFirstOrDefaultAsync<int>(
                    "SELECT COUNT(1) FROM Reviews WHERE ReviewId = @ReviewId AND UserId = @UserId",
                    new { ReviewId, UserId }
                );

                if (userOwnTheReview == 0)
                    return NotFound(
                        $"User with ID {UserId} does not own review with ID {ReviewId}."
                    );

                var query =
                    @"
            UPDATE Reviews 
            SET RestaurantId = @RestaurantId, 
                UserId = @UserId, 
                Comment = @Comment, 
                ReviewPhoto = @ReviewPhoto, 
                Rating = @Rating, 
                UpdatedAt = @UpdatedAt 
            WHERE ReviewId = @ReviewId";

                var parameters = new
                {
                    RestaurantId,
                    UserId,
                    reviewDto.Comment,
                    reviewDto.ReviewPhoto,
                    reviewDto.Rating,
                    UpdatedAt = DateTime.Now,
                    ReviewId,
                };

                await _dbConnection.ExecuteAsync(query, parameters);

                return Ok(reviewDto);
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("DeleteReview/{restaurantId:int}/{userId:int}/{reviewId:int}")]
        public async Task<IActionResult> DeleteReview(int restaurantId, int userId, int reviewId)
        {
            try
            {
                // Validate restaurant and review ownership
                var restaurantExists = await _dbConnection.QueryFirstOrDefaultAsync<int>(
                    "SELECT COUNT(1) FROM Restaurants WHERE RestaurantId = @restaurantId",
                    new { restaurantId }
                );
                var userOwnsReview = await _dbConnection.QueryFirstOrDefaultAsync<int>(
                    "SELECT COUNT(1) FROM Reviews WHERE ReviewId = @reviewId AND UserId = @userId",
                    new { reviewId, userId }
                );

                if (restaurantExists == 0)
                    return NotFound($"Restaurant ID {restaurantId} not found.");
                if (userOwnsReview == 0)
                    return NotFound($"User ID {userId} does not own review ID {reviewId}.");

                await _dbConnection.ExecuteAsync(
                    "DELETE FROM Reviews WHERE ReviewId = @reviewId",
                    new { reviewId }
                );
                return Ok("Review deleted");
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
