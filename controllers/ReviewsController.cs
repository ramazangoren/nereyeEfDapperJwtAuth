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
    public class ReviewsController : ControllerBase
    {
        private readonly NereyeDBContext _dapper;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(IConfiguration config, ILogger<ReviewsController> logger)
        {
            _dapper = new NereyeDBContext(config);
            _logger = logger;
        }

        // [AllowAnonymous]
        // [HttpGet("RestaurantReviews/{RestaurantId:int}")]
        // public ActionResult<IEnumerable<Reviews>> GetReviews(int RestaurantId)
        // {
        //     try
        //     {
        //         string sql = "SELECT * FROM Reviews WHERE RestaurantId =  " + RestaurantId;
        //         var reviews = _dapper.LoadData<Reviews>(sql);
        //         return reviews == null ? NotFound("No reviews found") : Ok(reviews);
        //     }
        //     catch (Exception ex)
        //     {
        //         return ErrorHandler.HandleError(ex, "GetReviews", _logger);
        //     }
        // }

        [AllowAnonymous]
        [HttpGet("RestaurantReviews/{RestaurantId:int}")]
        public ActionResult<IEnumerable<Reviews>> GetReviews(int RestaurantId)
        {
            try
            {
                // Ensure the RestaurantId is valid
                if (RestaurantId <= 0)
                    return BadRequest("Invalid RestaurantId.");

                string sql = "SELECT * FROM Reviews WHERE RestaurantId =  " + RestaurantId;
                var reviews = _dapper.LoadData<Reviews>(sql);

                if (reviews == null || !reviews.Any())
                {
                    return NotFound("No reviews found for this restaurant.");
                }

                return Ok(reviews);
            }
            catch (Exception ex)
            {
                // Use the ErrorHandler to handle the error
                return ErrorHandler.HandleError(ex, "GetReviews", _logger);
            }
        }

        [AllowAnonymous]
        [HttpGet("GetSingleReview/{restaurantId:int}/{reviewId:int}")]
        public ActionResult<Reviews> GetSingleReview(int restaurantId, int reviewId)
        {
            try
            {
                var singleReview = _dapper.LoadDataSingle<Reviews>(
                    "SELECT * FROM Reviews WHERE ReviewId = @reviewId AND RestaurantId = @restaurantId",
                    new { reviewId, restaurantId }
                );
                return singleReview == null ? NotFound("Review not found.") : Ok(singleReview);
            }
            catch (Exception ex)
            {
                return ErrorHandler.HandleError(ex, "GetSingleReview", _logger);
            }
        }

        [HttpPost("AddReview/{RestaurantId:int}")]
        public ActionResult AddReview(AddReviewDto reviewDto, int RestaurantId)
        {
            string UserId = User.FindFirst("userId")?.Value + ""; // Get the UserId from the token
            if (reviewDto == null)
                return BadRequest("Review data is required.");
            if (reviewDto.Rating < 1 || reviewDto.Rating > 5)
                return BadRequest("Rating must be between 1 and 5.");

            try
            {
                // Check if the restaurant exists
                int restaurantExists = _dapper.ExecuteSqlWithRowCount(
                    "SELECT COUNT(1) FROM Restaurants WHERE RestaurantId = @RestaurantId",
                    new { RestaurantId }
                );

                if (restaurantExists == 0)
                    return NotFound($"Restaurant with ID {RestaurantId} not found.");

                // Check if the user exists
                int userExists = _dapper.ExecuteSqlWithRowCount(
                    "SELECT COUNT(1) FROM Users WHERE UserId = @UserId",
                    new { UserId }
                );

                if (userExists == 0)
                    return NotFound($"User with ID {UserId} not found.");

                // Insert the review into the Reviews table
                var query =
                    @"
            INSERT INTO Reviews (RestaurantId, UserId, Comment, ReviewPhoto, Rating, CreatedAt) 
            VALUES (@RestaurantId, @UserId, @Comment, @ReviewPhoto, @Rating, @CreatedAt)";

                var parameters = new
                {
                    RestaurantId,
                    UserId,
                    reviewDto.Comment,
                    reviewDto.ReviewPhoto,
                    reviewDto.Rating,
                    CreatedAt = DateTime.Now,
                };

                // Execute the insert query
                bool result = _dapper.ExecuteSql(query, parameters);

                if (result)
                    return Ok("Review added successfully.");
                else
                    return BadRequest("Could not add the review.");
            }
            catch (Exception ex)
            {
                // Handle any unexpected exceptions
                return ErrorHandler.HandleError(ex, "AddReview", _logger);
            }
        }

        [HttpPut("UpdateReview/{RestaurantId:int}/{ReviewId:int}")]
        public ActionResult UpdateReview(
            UpdateReviewDto updateReviewDto,
            int RestaurantId,
            int ReviewId
        )
        {
            if (updateReviewDto == null)
                return BadRequest("Review data is required.");

            if (updateReviewDto.Rating < 1 || updateReviewDto.Rating > 5)
                return BadRequest("Rating must be between 1 and 5.");

            try
            {
                // Get the userId from the JWT token
                // string userId = User.FindFirst("userId")?.Value;
                string UserId = User.FindFirst("userId")?.Value + "";
                if (string.IsNullOrEmpty(UserId))
                {
                    return Unauthorized("User is not authenticated.");
                }

                int restaurantExists = _dapper.ExecuteSqlWithRowCount(
                    "SELECT COUNT(1) FROM Restaurants WHERE RestaurantId = @RestaurantId",
                    new { RestaurantId }
                );

                if (restaurantExists == 0)
                    return NotFound($"Restaurant with ID {RestaurantId} not found.");

                int userOwnTheReview = _dapper.ExecuteSqlWithRowCount(
                    "SELECT COUNT(1) FROM Reviews WHERE ReviewId = @ReviewId AND UserId = @UserId",
                    new { ReviewId, UserId }
                );

                if (userOwnTheReview == 0)
                    return NotFound(
                        $"User with ID {UserId} does not own review with ID {ReviewId}."
                    );

                var query =
                    @"UPDATE Reviews 
                    SET Comment = @Comment, ReviewPhoto = @ReviewPhoto, Rating = @Rating, UpdatedAt = @UpdatedAt 
                    WHERE ReviewId = @ReviewId";

                var parameters = new
                {
                    updateReviewDto.Comment,
                    updateReviewDto.ReviewPhoto,
                    updateReviewDto.Rating,
                    UpdatedAt = DateTime.Now,
                    ReviewId,
                };

                bool result = _dapper.ExecuteSql(query, parameters);

                return result
                    ? Ok("Review updated successfully.")
                    : BadRequest("Could not update the review.");
            }
            catch (Exception ex)
            {
                return ErrorHandler.HandleError(ex, "UpdateReview", _logger);
            }
        }

        [HttpDelete("DeleteReview/{restaurantId:int}/{reviewId:int}")]
        public IActionResult DeleteReview(int restaurantId, int reviewId)
        {
            try
            {
                string UserId = User.FindFirst("userId")?.Value + "";

                int restaurantExists = _dapper.ExecuteSqlWithRowCount(
                    "SELECT COUNT(1) FROM Restaurants WHERE RestaurantId = @restaurantId",
                    new { restaurantId }
                );

                if (restaurantExists == 0)
                    return NotFound($"Restaurant with ID {restaurantId} not found.");

                int userOwnsReview = _dapper.ExecuteSqlWithRowCount(
                    "SELECT COUNT(1) FROM Reviews WHERE ReviewId = @reviewId AND UserId = @userId",
                    new { reviewId, UserId }
                );

                if (userOwnsReview == 0)
                    return NotFound(
                        $"User with ID {UserId} does not own review with ID {reviewId}."
                    );

                bool result = _dapper.ExecuteSql(
                    "DELETE FROM Reviews WHERE ReviewId = @reviewId",
                    new { reviewId }
                );

                return result
                    ? Ok("Review deleted successfully.")
                    : BadRequest("Could not delete the review.");
            }
            catch (Exception ex)
            {
                return ErrorHandler.HandleError(ex, "DeleteReview", _logger);
            }
        }
    }
}
