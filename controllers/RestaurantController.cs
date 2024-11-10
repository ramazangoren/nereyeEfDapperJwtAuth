using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using api.DTOs.RestaurantDto;
using api.Models;
using api.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly IDbConnection _dbConnection;
        private readonly IOpenCloseService _openCloseService;

        public RestaurantController(IDbConnection dbConnection, IOpenCloseService openCloseService)
        {
            _dbConnection = dbConnection;
            _openCloseService = openCloseService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Restaurant>>> GetRestaurants()
        {
            var restaurants = await _dbConnection.QueryAsync<Restaurant>(
                "SELECT * FROM Restaurants WHERE IsActive = 1"
            );

            // Check open status for each restaurant
            foreach (var restaurant in restaurants)
            {
                var status = await _openCloseService.CheckIfOpenOrClosed(restaurant.RestaurantId);
                restaurant.OpenStatus = status;
                Console.WriteLine(status);
                Console.WriteLine(restaurant.OpenStatus);
            }

            return Ok(restaurants);
        }

        [HttpGet("GetSingleRestaurant")]
        public async Task<ActionResult<Restaurant>> GetSingleRestaurant(int RestaurantId)
        {
            var sql = "SELECT * FROM Restaurants WHERE RestaurantId = " + RestaurantId;
            var restaurant = await _dbConnection.QueryFirstOrDefaultAsync<Restaurant>(
                sql
            );
            if (restaurant == null)
            {
                return NotFound();
            }

            // Check open status for the single restaurant
            var status = await _openCloseService.CheckIfOpenOrClosed(restaurant.RestaurantId);
            restaurant.OpenStatus = status;

            return Ok(restaurant);
        }

        [HttpPost("CreateRestaurant/{userId}")]
        public async Task<ActionResult<int>> CreateRestaurant(
            CreateRestaurantDto restaurantDto,
            int userId
        )
        {
            if (restaurantDto == null)
                return BadRequest("Restaurant data is required.");

            restaurantDto.RestaurantCode = _random.Next(100000, 1000000);

            var query =
                @"
        INSERT INTO Restaurants (
            UserId, RestaurantName, RestaurantCode, FullAddress, Latitude, Longitude, 
            EstablishmentType, Phone, ExtraInfo, AboutUs, CreditCard, Cash, FoodCards, 
            AppleGooglePay, OutdoorSitting, PetsAllowed, Alcohol, Parking, OffersDelivery, 
            OffersTakeout, GoodForGroups, GoodForKids, FullBar, TakesReservation, 
            WaiterService, SelfService, HasTV, FreeWifi, StreetParking, BeerAndWineOnly, 
            Italian, Hookah, Burger, HotDogs, FastFood, Breakfast, Doner, HalalFood, 
            ImageUrls, MondayOpens, MondayCloses, TuesdayOpens, TuesdayCloses, 
            WednesdayOpens, WednesdayCloses, ThursdayOpens, ThursdayCloses, 
            FridayOpens, FridayCloses, SaturdayOpens, SaturdayCloses, SundayOpens, 
            SundayCloses, CreatedAt, UpdatedAt, IsActive
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
            @CreatedAt, @UpdatedAt, @IsActive
        );
        SELECT CAST(SCOPE_IDENTITY() AS int);";

            var parameters = new
            {
                UserId = userId,
                restaurantDto.RestaurantName,
                restaurantDto.RestaurantCode,
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
                restaurantDto.IsActive,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            await _dbConnection.QuerySingleAsync<int>(query, parameters);
            return Ok("restaurant created");
        }

        [HttpPut("UpdateRestaurant/{RestaurantId}")]
        public async Task<ActionResult> UpdateRestaurant(
            [FromBody] UpdateRestaurantDto restaurantDto,
            int RestaurantId,
            int UserId
        )
        {
            if (restaurantDto == null)
                return BadRequest("Restaurant data is required.");

            var restaurantExistsQuery =
                @" SELECT COUNT(*) FROM Restaurants WHERE RestaurantId = @RestaurantId AND UserId = @UserId";

            var restaurantCount = await _dbConnection.ExecuteScalarAsync<int>(
                restaurantExistsQuery,
                new { RestaurantId, UserId }
            );

            if (restaurantCount == 0)
            {
                return NotFound(
                    $"Restaurant with ID {RestaurantId} not found for UserId = {UserId}."
                );
            }

            var updateQuery =
                @"
        UPDATE Restaurants 
        SET RestaurantName = @RestaurantName,
            FullAddress = @FullAddress,
            Latitude = @Latitude,
            Longitude = @Longitude,
            EstablishmentType = @EstablishmentType,
            Phone = @Phone,
            ExtraInfo = @ExtraInfo,
            AboutUs = @AboutUs,
            CreditCard = @CreditCard,
            Cash = @Cash,
            FoodCards = @FoodCards,
            AppleGooglePay = @AppleGooglePay,
            OutdoorSitting = @OutdoorSitting,
            PetsAllowed = @PetsAllowed,
            Alcohol = @Alcohol,
            Parking = @Parking,
            OffersDelivery = @OffersDelivery,
            OffersTakeout = @OffersTakeout,
            GoodForGroups = @GoodForGroups,
            GoodForKids = @GoodForKids,
            FullBar = @FullBar,
            TakesReservation = @TakesReservation,
            WaiterService = @WaiterService,
            SelfService = @SelfService,
            HasTV = @HasTV,
            FreeWifi = @FreeWifi,
            StreetParking = @StreetParking,
            BeerAndWineOnly = @BeerAndWineOnly,
            Italian = @Italian,
            Hookah = @Hookah,
            Burger = @Burger,
            HotDogs = @HotDogs,
            FastFood = @FastFood,
            Breakfast = @Breakfast,
            Doner = @Doner,
            HalalFood = @HalalFood,
            ImageUrls = @ImageUrls,
            MondayOpens = @MondayOpens,
            MondayCloses = @MondayCloses,
            TuesdayOpens = @TuesdayOpens,
            TuesdayCloses = @TuesdayCloses,
            WednesdayOpens = @WednesdayOpens,
            WednesdayCloses = @WednesdayCloses,
            ThursdayOpens = @ThursdayOpens,
            ThursdayCloses = @ThursdayCloses,
            FridayOpens = @FridayOpens,
            FridayCloses = @FridayCloses,
            SaturdayOpens = @SaturdayOpens,
            SaturdayCloses = @SaturdayCloses,
            SundayOpens = @SundayOpens,
            SundayCloses = @SundayCloses,
            UpdatedAt = @UpdatedAt,
            IsActive = @IsActive
        WHERE RestaurantId = @RestaurantId";

            // Execute the update
            var affectedRows = await _dbConnection.ExecuteAsync(
                updateQuery,
                new
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
                    restaurantDto.IsActive,
                    RestaurantId // Add RestaurantId here
                    ,
                }
            );

            if (affectedRows == 0)
            {
                return NotFound($"Restaurant with ID {RestaurantId} could not be updated.");
            }

            return Ok("updated the restaurant");
        }

        [HttpDelete("DeleteRestaurant/{RestaurantId}")]
        public async Task<ActionResult> DeleteRestaurant(int RestaurantId, int UserId)
        {
            var restaurantCount = await _dbConnection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Restaurants WHERE RestaurantId = @RestaurantId AND UserId =  @UserId",
                new { RestaurantId, UserId }
            );

            if (restaurantCount == 0)
            {
                return NotFound(
                    $"Restaurant with ID {RestaurantId} not found for UserId = {UserId}."
                );
            }
            var deleteQuery = "DELETE FROM Restaurants WHERE RestaurantId = " + RestaurantId;
            var rowsAffected = await _dbConnection.ExecuteAsync(deleteQuery);

            return rowsAffected > 0 ? Ok("restaurant deleted") : NotFound();
        }

        private static readonly Random _random = new Random();
    }
}
