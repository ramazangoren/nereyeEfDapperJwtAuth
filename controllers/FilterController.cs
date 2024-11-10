// using System;
// using System.Collections.Generic;
// using System.Data;
// using System.Linq;
// using System.Threading.Tasks;
// using api.Data;
// using api.DTOs.RestaurantDto;
// using api.Models;
// using api.Services;
// using Dapper;
// using Microsoft.AspNetCore.Mvc;

// namespace api.Controllers
// {
//     [Route("api/[controller]")]
//     [ApiController]
//     public class FilterController : ControllerBase
//     {
//         private readonly NereyeDBContext _dapper;
//         private readonly ILogger<FilterController> _logger;
//         private readonly IOpenCloseService _openCloseService;

//         public FilterController(
//             IConfiguration config,
//             ILogger<FilterController> logger,
//             IOpenCloseService openCloseService
//         )
//         {
//             _dapper = new NereyeDBContext(config);
//             _logger = logger;
//             _openCloseService = openCloseService;
//         }

//         [HttpGet("Filter")]
//         public ActionResult<IEnumerable<Restaurant>> Filter([FromQuery] RestaurantFilter filter)
//         {
//             var (query, parameters) = BuildQuery(filter);

//             var restaurants = _dapper.LoadDataFilterAndSearch<Restaurant>(query, parameters);

//             if (!restaurants.Any())
//             {
//                 return NotFound("No restaurants found matching the criteria.");
//             }

//             // Set OpenStatus for each restaurant
//             foreach (var restaurant in restaurants)
//             {
//                 restaurant.OpenStatus = _openCloseService.CheckIfOpenOrClosed(
//                     restaurant.RestaurantId
//                 );
//             }

//             // Optionally, filter based on OpenStatus if specified in the filter
//             if (filter.OpenStatus.HasValue)
//             {
//                 restaurants = restaurants
//                     .Where(r => r.OpenStatus == filter.OpenStatus.Value)
//                     .ToList();
//             }

//             return Ok(restaurants);
//         }

//         private (string, DynamicParameters) BuildQuery(RestaurantFilter filter)
//         {
//             var query = "SELECT * FROM Restaurants WHERE 1=1";
//             var parameters = new DynamicParameters();

//             foreach (var property in typeof(RestaurantFilter).GetProperties())
//             {
//                 var value = property.GetValue(filter);
//                 if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
//                 {
//                     continue; // Skip null or empty string properties
//                 }

//                 string columnName = property.Name;

//                 // Skip the OpenStatus property from the query
//                 if (columnName == "OpenStatus")
//                     continue;

//                 // If the property is Rating, we may want to filter differently
//                 if (columnName == "Rating")
//                 {
//                     query +=
//                         $" AND RestaurantId IN (SELECT RestaurantId FROM Reviews WHERE Rating = @{columnName})";
//                 }
//                 else
//                 {
//                     query += $" AND {columnName} = @{columnName}";
//                 }

//                 parameters.Add(columnName, value);
//             }

//             return (query, parameters);
//         }
//     }
// }


using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.DTOs.RestaurantDto;
using api.Models;
using api.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using api.Helpers;

namespace api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FilterController : ControllerBase
    {
        private readonly NereyeDBContext _dapper;
        private readonly ILogger<FilterController> _logger;
        private readonly IOpenCloseService _openCloseService;

        public FilterController(
            IConfiguration config,
            ILogger<FilterController> logger,
            IOpenCloseService openCloseService
        )
        {
            _dapper = new NereyeDBContext(config);
            _logger = logger;
            _openCloseService = openCloseService;
        }

        [HttpGet("Filter")]
        public ActionResult<IEnumerable<Restaurant>> Filter([FromQuery] RestaurantFilter filter)
        {
            try
            {
                var (query, parameters) = BuildQuery(filter);

                var restaurants = _dapper.LoadDataFilterAndSearch<Restaurant>(query, parameters);

                if (!restaurants.Any())
                {
                    return NotFound("No restaurants found matching the criteria.");
                }

                // Set OpenStatus for each restaurant
                foreach (var restaurant in restaurants)
                {
                    restaurant.OpenStatus = _openCloseService.CheckIfOpenOrClosed(
                        restaurant.RestaurantId
                    );
                }

                // Optionally, filter based on OpenStatus if specified in the filter
                if (filter.OpenStatus.HasValue)
                {
                    restaurants = restaurants
                        .Where(r => r.OpenStatus == filter.OpenStatus.Value)
                        .ToList();
                }

                return Ok(restaurants);
            }
            catch (Exception ex)
            {
                // Handle the error using the ErrorHandler class
                return ErrorHandler.HandleError(ex, "Filtering restaurants", _logger);
            }
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

                // Skip the OpenStatus property from the query
                if (columnName == "OpenStatus")
                    continue;

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
