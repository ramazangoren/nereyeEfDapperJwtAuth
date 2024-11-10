using System;
using System.Data;
using System.Threading.Tasks;
using api.Data;
using api.DTOs.RestaurantDto;
using api.Services;
using Dapper;

public class OpenCloseService : IOpenCloseService
{
    private readonly NereyeDBContext _dapper;

    public OpenCloseService(IConfiguration config)
    {
        _dapper = new NereyeDBContext(config);
    }

    public bool? CheckIfOpenOrClosed(int RestaurantId)
    {
        string sql = @"
            SELECT 
                MondayOpens, MondayCloses,
                TuesdayOpens, TuesdayCloses,
                WednesdayOpens, WednesdayCloses,
                ThursdayOpens, ThursdayCloses,
                FridayOpens, FridayCloses,
                SaturdayOpens, SaturdayCloses,
                SundayOpens, SundayCloses
            FROM Restaurants
            WHERE RestaurantId = " + RestaurantId;

        // Using an anonymous object for the parameter
        var queryResult = _dapper.LoadData<RestaurantHours>(sql).FirstOrDefault();

        if (queryResult == null)
        {
            return null;
        }

        var currentDay = DateTime.Now.DayOfWeek;
        var currentTime = DateTime.Now.TimeOfDay;

        TimeSpan? opensTime = currentDay switch
        {
            DayOfWeek.Monday => queryResult.MondayOpens,
            DayOfWeek.Tuesday => queryResult.TuesdayOpens,
            DayOfWeek.Wednesday => queryResult.WednesdayOpens,
            DayOfWeek.Thursday => queryResult.ThursdayOpens,
            DayOfWeek.Friday => queryResult.FridayOpens,
            DayOfWeek.Saturday => queryResult.SaturdayOpens,
            DayOfWeek.Sunday => queryResult.SundayOpens,
            _ => null,
        };

        TimeSpan? closesTime = currentDay switch
        {
            DayOfWeek.Monday => queryResult.MondayCloses,
            DayOfWeek.Tuesday => queryResult.TuesdayCloses,
            DayOfWeek.Wednesday => queryResult.WednesdayCloses,
            DayOfWeek.Thursday => queryResult.ThursdayCloses,
            DayOfWeek.Friday => queryResult.FridayCloses,
            DayOfWeek.Saturday => queryResult.SaturdayCloses,
            DayOfWeek.Sunday => queryResult.SundayCloses,
            _ => null,
        };

        if (opensTime.HasValue && closesTime.HasValue)
        {
            return currentTime >= opensTime.Value && currentTime <= closesTime.Value;
        }

        return false;
    }
}
