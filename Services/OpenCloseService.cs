// using System;
// using System.Threading.Tasks;
// using Dapper;
// using System.Data;
// using api.Services;
// using api.DTOs.RestaurantDto;

// public class OpenCloseService : IOpenCloseService
// {
//     private readonly IDbConnection _dbConnection;

//     public OpenCloseService(IDbConnection dbConnection)
//     {
//         _dbConnection = dbConnection;
//     }

//     public async Task<string> CheckIfOpenOrClosed(int restaurantId)
//     {
//         var queryResult = await _dbConnection.QuerySingleOrDefaultAsync<RestaurantHours>(
//             @"SELECT
//                 MondayOpens, MondayCloses,
//                 TuesdayOpens, TuesdayCloses,
//                 WednesdayOpens, WednesdayCloses,
//                 ThursdayOpens, ThursdayCloses,
//                 FridayOpens, FridayCloses,
//                 SaturdayOpens, SaturdayCloses,
//                 SundayOpens, SundayCloses
//             FROM
//                 Restaurants
//             WHERE
//                 RestaurantId = @RestaurantId",
//             new { RestaurantId = restaurantId }
//         );

//         if (queryResult == null)
//         {
//             return "Not Found";
//         }

//         var restaurantHours = queryResult;

//         var currentDay = DateTime.Now.DayOfWeek;
//         var currentTime = DateTime.Now.TimeOfDay;

//         TimeSpan? opensTime = currentDay switch
//         {
//             DayOfWeek.Monday => restaurantHours.MondayOpens,
//             DayOfWeek.Tuesday => restaurantHours.TuesdayOpens,
//             DayOfWeek.Wednesday => restaurantHours.WednesdayOpens,
//             DayOfWeek.Thursday => restaurantHours.ThursdayOpens,
//             DayOfWeek.Friday => restaurantHours.FridayOpens,
//             DayOfWeek.Saturday => restaurantHours.SaturdayOpens,
//             DayOfWeek.Sunday => restaurantHours.SundayOpens,
//             _ => null,
//         };

//         TimeSpan? closesTime = currentDay switch
//         {
//             DayOfWeek.Monday => restaurantHours.MondayCloses,
//             DayOfWeek.Tuesday => restaurantHours.TuesdayCloses,
//             DayOfWeek.Wednesday => restaurantHours.WednesdayCloses,
//             DayOfWeek.Thursday => restaurantHours.ThursdayCloses,
//             DayOfWeek.Friday => restaurantHours.FridayCloses,
//             DayOfWeek.Saturday => restaurantHours.SaturdayCloses,
//             DayOfWeek.Sunday => restaurantHours.SundayCloses,
//             _ => null,
//         };

//         if (opensTime.HasValue && closesTime.HasValue)
//         {
//             if (currentTime >= opensTime.Value && currentTime <= closesTime.Value)
//             {
//                 return "Open";
//             }
//         }

//         return "Closed";
//     }
// }

using System;
using System.Data;
using System.Threading.Tasks;
using api.DTOs.RestaurantDto;
using api.Services;
using Dapper;

public class OpenCloseService : IOpenCloseService
{
    private readonly IDbConnection _dbConnection;

    public OpenCloseService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<bool?> CheckIfOpenOrClosed(int restaurantId)
    {
        var queryResult = await _dbConnection.QuerySingleOrDefaultAsync<RestaurantHours>(
            @"SELECT 
                MondayOpens, MondayCloses,
                TuesdayOpens, TuesdayCloses,
                WednesdayOpens, WednesdayCloses,
                ThursdayOpens, ThursdayCloses,
                FridayOpens, FridayCloses,
                SaturdayOpens, SaturdayCloses,
                SundayOpens, SundayCloses
            FROM 
                Restaurants
            WHERE 
                RestaurantId = @RestaurantId",
            new { RestaurantId = restaurantId }
        );

        if (queryResult == null)
        {
            return null; // Return null to indicate not found
        }

        var restaurantHours = queryResult;

        var currentDay = DateTime.Now.DayOfWeek;
        var currentTime = DateTime.Now.TimeOfDay;

        TimeSpan? opensTime = currentDay switch
        {
            DayOfWeek.Monday => restaurantHours.MondayOpens,
            DayOfWeek.Tuesday => restaurantHours.TuesdayOpens,
            DayOfWeek.Wednesday => restaurantHours.WednesdayOpens,
            DayOfWeek.Thursday => restaurantHours.ThursdayOpens,
            DayOfWeek.Friday => restaurantHours.FridayOpens,
            DayOfWeek.Saturday => restaurantHours.SaturdayOpens,
            DayOfWeek.Sunday => restaurantHours.SundayOpens,
            _ => null,
        };

        TimeSpan? closesTime = currentDay switch
        {
            DayOfWeek.Monday => restaurantHours.MondayCloses,
            DayOfWeek.Tuesday => restaurantHours.TuesdayCloses,
            DayOfWeek.Wednesday => restaurantHours.WednesdayCloses,
            DayOfWeek.Thursday => restaurantHours.ThursdayCloses,
            DayOfWeek.Friday => restaurantHours.FridayCloses,
            DayOfWeek.Saturday => restaurantHours.SaturdayCloses,
            DayOfWeek.Sunday => restaurantHours.SundayCloses,
            _ => null,
        };

        if (opensTime.HasValue && closesTime.HasValue)
        {
            return currentTime >= opensTime.Value && currentTime <= closesTime.Value;
        }

        return false; // Default to false if hours are not available
    }
}
