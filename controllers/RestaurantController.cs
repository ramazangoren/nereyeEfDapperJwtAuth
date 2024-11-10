using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using api.Data;
using api.DTOs.RestaurantDto;
using api.Helpers;
using api.Models;
using api.Services;
using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class RestaurantController : ControllerBase
    {
        private readonly NereyeDBContext _dapper;
        private static readonly Random _random = new Random();
        private readonly ILogger<RestaurantController> _logger;
        private readonly IOpenCloseService _openCloseService;

        public RestaurantController(
            IConfiguration config,
            ILogger<RestaurantController> logger,
            IOpenCloseService openCloseService
        )
        {
            _dapper = new NereyeDBContext(config);
            _logger = logger;
            _openCloseService = openCloseService;
        }

        [AllowAnonymous]
        [HttpGet("GetRestaurants")]
        public ActionResult<IEnumerable<Restaurant>> GetRestaurants()
        {
            try
            {
                string sql = "SELECT * FROM dbo.Restaurants WHERE Active = 1";
                var restaurants = _dapper.LoadData<Restaurant>(sql);

                foreach (var restaurant in restaurants)
                {
                    var status = _openCloseService.CheckIfOpenOrClosed(restaurant.RestaurantId);
                    restaurant.OpenStatus = status;
                    Console.WriteLine($"Status: {status}");
                }

                return Ok(restaurants);
            }
            catch (Exception ex)
            {
                return ErrorHandler.HandleError(ex, "GetRestaurants", _logger);
            }
        }

        [AllowAnonymous]
        [HttpGet("GetSingleRestaurants/{RestaurantId}")]
        public ActionResult<IEnumerable<Restaurant>> GetSingleRestaurants(int RestaurantId)
        {
            try
            {
                string sql = "SELECT * FROM dbo.Restaurants WHERE RestaurantId = @RestaurantId";
                var parameter = new { RestaurantId };
                var restaurant = _dapper.LoadDataSingle<Restaurant>(sql, parameter);
                if (restaurant == null)
                {
                    return NotFound();
                }
                var status = _openCloseService.CheckIfOpenOrClosed(restaurant.RestaurantId);
                restaurant.OpenStatus = status;
                return Ok(restaurant);
            }
            catch (Exception ex)
            {
                return ErrorHandler.HandleError(ex, "GetSingleRestaurants", _logger);
            }
        }

        [HttpPost("CreateRestaurant")]
        public ActionResult<int> CreateRestaurant(CreateRestaurantDto restaurantDto)
        {
            if (restaurantDto == null)
                return BadRequest("Restaurant data is required.");

            try
            {
                string UserId = User.FindFirst("userId")?.Value + "";
                var userExistsQuery = "SELECT COUNT(1) FROM dbo.Users WHERE UserId = @UserId";
                int userExists = _dapper.ExecuteSqlWithRowCount(userExistsQuery, new { UserId });

                if (userExists == 0)
                {
                    return BadRequest("User not found.");
                }

                var query =
                    @"
            INSERT INTO dbo.Restaurants (
                UserId, RestaurantName, RestaurantCode, FullAddress, Latitude, Longitude, 
                EstablishmentType, Phone, ExtraInfo, AboutUs, CreditCard, Cash, FoodCards, 
                AppleGooglePay, OutdoorSitting, PetsAllowed, Alcohol, Parking, OffersDelivery, 
                OffersTakeout, GoodForGroups, GoodForKids, FullBar, TakesReservation, 
                WaiterService, SelfService, HasTV, FreeWifi, StreetParking, BeerAndWineOnly, 
                Italian, Hookah, Burger, HotDogs, FastFood, Breakfast, Doner, HalalFood, 
                ImageUrls, MondayOpens, MondayCloses, TuesdayOpens, TuesdayCloses, 
                WednesdayOpens, WednesdayCloses, ThursdayOpens, ThursdayCloses, 
                FridayOpens, FridayCloses, SaturdayOpens, SaturdayCloses, SundayOpens, 
                SundayCloses, CreatedAt, UpdatedAt, Active
            ) VALUES (
                @UserId, @RestaurantName, @RestaurantCode, @FullAddress, @Latitude, 
                @Longitude, @EstablishmentType, @Phone, @ExtraInfo, @AboutUs, 
                @CreditCard, @Cash, @FoodCards, @AppleGooglePay, @OutdoorSitting, 
                @PetsAllowed, @Alcohol, @Parking, @OffersDelivery, @OffersTakeout, 
                @GoodForGroups, @GoodForKids, @FullBar, @TakesReservation, 
                @WaiterService, @SelfService, @HasTV, @FreeWifi, @StreetParking, 
                @BeerAndWineOnly, @Italian, @Hookah, @Burger, @HotDogs, @FastFood, 
                @Breakfast, @Doner, @HalalFood, @ImageUrls, 
                @MondayOpens, @MondayCloses, @TuesdayOpens, @TuesdayCloses, 
                @WednesdayOpens, @WednesdayCloses, @ThursdayOpens, 
                @ThursdayCloses, @FridayOpens, @FridayCloses, @SaturdayOpens, 
                @SaturdayCloses, @SundayOpens, @SundayCloses, 
                @CreatedAt, @UpdatedAt, @Active
            );
            SELECT CAST(SCOPE_IDENTITY() AS int);";

                var RestaurantCode = _random.Next(100000, 1000000);

                var parameters = new
                {
                    UserId,
                    restaurantDto.RestaurantName,
                    RestaurantCode,
                    restaurantDto.FullAddress,
                    restaurantDto.Latitude,
                    restaurantDto.Longitude,
                    restaurantDto.EstablishmentType,
                    restaurantDto.Phone,
                    restaurantDto.ExtraInfo,
                    restaurantDto.AboutUs,
                    restaurantDto.CreditCard,
                    restaurantDto.Cash,
                    restaurantDto.FoodCards,
                    restaurantDto.AppleGooglePay,
                    restaurantDto.OutdoorSitting,
                    restaurantDto.PetsAllowed,
                    restaurantDto.Alcohol,
                    restaurantDto.Parking,
                    restaurantDto.OffersDelivery,
                    restaurantDto.OffersTakeout,
                    restaurantDto.GoodForGroups,
                    restaurantDto.GoodForKids,
                    restaurantDto.FullBar,
                    restaurantDto.TakesReservation,
                    restaurantDto.WaiterService,
                    restaurantDto.SelfService,
                    restaurantDto.HasTV,
                    restaurantDto.FreeWifi,
                    restaurantDto.StreetParking,
                    restaurantDto.BeerAndWineOnly,
                    restaurantDto.Italian,
                    restaurantDto.Hookah,
                    restaurantDto.Burger,
                    restaurantDto.HotDogs,
                    restaurantDto.FastFood,
                    restaurantDto.Breakfast,
                    restaurantDto.Doner,
                    restaurantDto.HalalFood,
                    restaurantDto.ImageUrls,
                    restaurantDto.MondayOpens,
                    restaurantDto.MondayCloses,
                    restaurantDto.TuesdayOpens,
                    restaurantDto.TuesdayCloses,
                    restaurantDto.WednesdayOpens,
                    restaurantDto.WednesdayCloses,
                    restaurantDto.ThursdayOpens,
                    restaurantDto.ThursdayCloses,
                    restaurantDto.FridayOpens,
                    restaurantDto.FridayCloses,
                    restaurantDto.SaturdayOpens,
                    restaurantDto.SaturdayCloses,
                    restaurantDto.SundayOpens,
                    restaurantDto.SundayCloses,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    restaurantDto.Active,
                };

                bool result = _dapper.ExecuteSql(query, parameters);

                return result
                    ? Ok($"Restaurant created successfully.")
                    : StatusCode(500, "Restaurant creation failed.");
            }
            catch (Exception ex)
            {
                return ErrorHandler.HandleError(ex, "CreateRestaurant", _logger);
            }
        }

        [HttpPut("UpdateRestaurant/{RestaurantId}")]
        public ActionResult UpdateRestaurant(
            [FromBody] UpdateRestaurantDto restaurantDto,
            int RestaurantId
        )
        {
            if (restaurantDto == null)
                return BadRequest("Restaurant data is required.");

            try
            {
                string UserId = User.FindFirst("userId")?.Value + "";
                // Check if the restaurant exists and the user owns the restaurant
                var restaurantExistsQuery =
                    @"SELECT COUNT(*) FROM dbo.Restaurants WHERE RestaurantId = @RestaurantId AND UserId = @UserId";

                int restaurantCount = _dapper.ExecuteSqlWithRowCount(
                    restaurantExistsQuery,
                    new { RestaurantId, UserId }
                );

                // If the restaurant does not exist or the user does not own it
                if (restaurantCount == 0)
                {
                    return NotFound(
                        $"Restaurant with ID {RestaurantId} not found for UserId = {UserId}."
                    );
                }

                // Update query to modify restaurant details
                var updateQuery =
                    @"UPDATE dbo.Restaurants 
            SET RestaurantName = @RestaurantName, FullAddress = @FullAddress, Latitude = @Latitude, Longitude = @Longitude,
                EstablishmentType = @EstablishmentType, Phone = @Phone, ExtraInfo = @ExtraInfo, AboutUs = @AboutUs, CreditCard = @CreditCard,
                Cash = @Cash, FoodCards = @FoodCards, AppleGooglePay = @AppleGooglePay, OutdoorSitting = @OutdoorSitting,
                PetsAllowed = @PetsAllowed, Alcohol = @Alcohol, Parking = @Parking, OffersDelivery = @OffersDelivery,
                OffersTakeout = @OffersTakeout, GoodForGroups = @GoodForGroups, GoodForKids = @GoodForKids, FullBar = @FullBar,
                TakesReservation = @TakesReservation, WaiterService = @WaiterService, SelfService = @SelfService, HasTV = @HasTV,
                FreeWifi = @FreeWifi, StreetParking = @StreetParking, BeerAndWineOnly = @BeerAndWineOnly, Italian = @Italian,
                Hookah = @Hookah, Burger = @Burger, HotDogs = @HotDogs, FastFood = @FastFood, Breakfast = @Breakfast,
                Doner = @Doner, HalalFood = @HalalFood, ImageUrls = @ImageUrls, MondayOpens = @MondayOpens, MondayCloses = @MondayCloses,
                TuesdayOpens = @TuesdayOpens, TuesdayCloses = @TuesdayCloses, WednesdayOpens = @WednesdayOpens, 
                WednesdayCloses = @WednesdayCloses, ThursdayOpens = @ThursdayOpens, ThursdayCloses = @ThursdayCloses,
                FridayOpens = @FridayOpens, FridayCloses = @FridayCloses, SaturdayOpens = @SaturdayOpens, 
                SaturdayCloses = @SaturdayCloses, SundayOpens = @SundayOpens, SundayCloses = @SundayCloses, 
                UpdatedAt = @UpdatedAt
            WHERE RestaurantId = @RestaurantId AND UserId = @UserId";

                var parameters = new
                {
                    restaurantDto.RestaurantName,
                    restaurantDto.FullAddress,
                    restaurantDto.Latitude,
                    restaurantDto.Longitude,
                    restaurantDto.EstablishmentType,
                    restaurantDto.Phone,
                    restaurantDto.ExtraInfo,
                    restaurantDto.AboutUs,
                    restaurantDto.CreditCard,
                    restaurantDto.Cash,
                    restaurantDto.FoodCards,
                    restaurantDto.AppleGooglePay,
                    restaurantDto.OutdoorSitting,
                    restaurantDto.PetsAllowed,
                    restaurantDto.Alcohol,
                    restaurantDto.Parking,
                    restaurantDto.OffersDelivery,
                    restaurantDto.OffersTakeout,
                    restaurantDto.GoodForGroups,
                    restaurantDto.GoodForKids,
                    restaurantDto.FullBar,
                    restaurantDto.TakesReservation,
                    restaurantDto.WaiterService,
                    restaurantDto.SelfService,
                    restaurantDto.HasTV,
                    restaurantDto.FreeWifi,
                    restaurantDto.StreetParking,
                    restaurantDto.BeerAndWineOnly,
                    restaurantDto.Italian,
                    restaurantDto.Hookah,
                    restaurantDto.Burger,
                    restaurantDto.HotDogs,
                    restaurantDto.FastFood,
                    restaurantDto.Breakfast,
                    restaurantDto.Doner,
                    restaurantDto.HalalFood,
                    restaurantDto.ImageUrls,
                    restaurantDto.MondayOpens,
                    restaurantDto.MondayCloses,
                    restaurantDto.TuesdayOpens,
                    restaurantDto.TuesdayCloses,
                    restaurantDto.WednesdayOpens,
                    restaurantDto.WednesdayCloses,
                    restaurantDto.ThursdayOpens,
                    restaurantDto.ThursdayCloses,
                    restaurantDto.FridayOpens,
                    restaurantDto.FridayCloses,
                    restaurantDto.SaturdayOpens,
                    restaurantDto.SaturdayCloses,
                    restaurantDto.SundayOpens,
                    restaurantDto.SundayCloses,
                    UpdatedAt = DateTime.Now,
                    RestaurantId,
                    UserId,
                };

                bool restaurantUpdated = _dapper.ExecuteSql(updateQuery, parameters);

                if (restaurantUpdated)
                {
                    return Ok("Restaurant updated successfully.");
                }
                else
                {
                    return StatusCode(500, "Restaurant update failed.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception and return a generic error message
                return ErrorHandler.HandleError(ex, "UpdateRestaurant", _logger);
            }
        }

        // [HttpDelete("DeleteRestaurant/{RestaurantId}")]
        // public ActionResult DeleteRestaurant(int RestaurantId)
        // {
        //     try
        //     {
        //         string UserId = User.FindFirst("userId")?.Value + "";
        //         var restaurantExistsQuery =
        //             @"SELECT COUNT(*) FROM dbo.Restaurants WHERE RestaurantId = @RestaurantId AND UserId = @UserId";
        //         int restaurantCount = _dapper.ExecuteSqlWithRowCount(
        //             restaurantExistsQuery,
        //             new { RestaurantId, UserId }
        //         );

        //         if (restaurantCount == 0)
        //         {
        //             return NotFound("Restaurant not found for the specified User.");
        //         }

        //         string deleteQuery =
        //             "DELETE FROM dbo.Restaurants WHERE RestaurantId = @RestaurantId AND UserId = @UserId";
        //         var parameters = new { RestaurantId, UserId };

        //         bool restaurantDeleted = _dapper.ExecuteSql(deleteQuery, parameters);

        //         if (restaurantDeleted)
        //         {
        //             return Ok("Restaurant deleted successfully.");
        //         }
        //         else
        //         {
        //             return StatusCode(500, "Restaurant deletion failed.");
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return ErrorHandler.HandleError(ex, "DeleteRestaurant", _logger);
        //     }
        // }
        
        [HttpDelete("DeleteRestaurant/{RestaurantId}")]
public ActionResult DeleteRestaurant(int RestaurantId)
{
    try
    {
        // string UserId = User.FindFirst("userId")?.Value;
        string UserId = User.FindFirst("userId")?.Value + "";
        if (string.IsNullOrEmpty(UserId))
        {
            return Unauthorized("User ID not found in token.");
        }

        // Check if the restaurant exists and belongs to the user
        var restaurantExistsQuery =
            @"SELECT COUNT(1) FROM dbo.Restaurants WHERE RestaurantId = @RestaurantId AND UserId = @UserId";
        int restaurantCount = _dapper.ExecuteSqlWithRowCount(
            restaurantExistsQuery,
            new { RestaurantId, UserId }
        );

        if (restaurantCount == 0)
        {
            return NotFound("Restaurant not found or does not belong to the specified user.");
        }

        // Attempt to delete the restaurant
        string deleteQuery = "DELETE FROM dbo.Restaurants WHERE RestaurantId = @RestaurantId AND UserId = @UserId";
        var parameters = new { RestaurantId, UserId };

        bool restaurantDeleted = _dapper.ExecuteSql(deleteQuery, parameters);

        if (restaurantDeleted)
        {
            return Ok("Restaurant deleted successfully.");
        }
        else
        {
            return StatusCode(500, "Failed to delete the restaurant.");
        }
    }
    catch (Exception ex)
    {
        // Use the ErrorHandler to log the error and return a 500 error response
        return ErrorHandler.HandleError(ex, "DeleteRestaurant", _logger);
    }
}

    }
}
