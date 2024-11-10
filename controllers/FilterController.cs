using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    public class FilterController : ControllerBase
    {
        private readonly IDbConnection _dbConnection;
        private readonly IOpenCloseService _openCloseService;

        public FilterController(IDbConnection dbConnection, IOpenCloseService openCloseService)
        {
            _dbConnection = dbConnection;
            _openCloseService = openCloseService;
        }

        [HttpGet("Filter")]
        public async Task<ActionResult<IEnumerable<Restaurant>>> Search(
            [FromQuery] RestaurantFilter filter
        )
        {
            var (query, parameters) = BuildQuery(filter);

            var restaurants = await _dbConnection.QueryAsync<Restaurant>(query, parameters);

            if (!restaurants.Any())
            {
                return NotFound("No restaurants found matching the criteria.");
            }

            return Ok(restaurants);
        }

        private (string, DynamicParameters) BuildQuery(RestaurantFilter filter)
        {
            var query = "SELECT * FROM Restaurants WHERE 1=1";
            var parameters = new DynamicParameters();

            foreach (var property in typeof(RestaurantFilter).GetProperties())
            {
                var value = property.GetValue(filter);
                if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                {
                    continue; // Skip null or empty string properties
                }

                string columnName = property.Name;
                // If the property is Rating, we may want to filter differently
                if (columnName == "Rating")
                {
                    query +=
                        $" AND RestaurantId IN (SELECT RestaurantId FROM Reviews WHERE Rating = @{columnName})";
                }
                else
                {
                    query += $" AND {columnName} = @{columnName}";
                }

                parameters.Add(columnName, value);
            }

            return (query, parameters);
        }
    }
}
