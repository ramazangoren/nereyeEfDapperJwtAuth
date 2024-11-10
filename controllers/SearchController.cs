using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Models;
using api.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using api.Helpers;

namespace api.controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly NereyeDBContext _dapper;
        private readonly ILogger<SearchController> _logger;
        private readonly IOpenCloseService _openCloseService;

        public SearchController(
            IConfiguration config,
            ILogger<SearchController> logger,
            IOpenCloseService openCloseService
        )
        {
            _dapper = new NereyeDBContext(config);
            _logger = logger;
            _openCloseService = openCloseService;
        }

        [HttpGet("Search")]
        public ActionResult<IEnumerable<object>> Search([FromQuery] string searchParam)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchParam))
                    return BadRequest("Search parameter is required.");

                string SearchTerm = $"%{searchParam}%";

                // Search for restaurants by name
                var restaurantResults = _dapper.LoadDataFilterAndSearch<object>(
                    @"SELECT 
                        r.RestaurantId,
                        r.RestaurantName,
                        r.FullAddress,
                        r.Phone,
                        r.EstablishmentType
                    FROM 
                        Restaurants r
                    WHERE 
                        r.RestaurantName LIKE @SearchTerm",
                    new { SearchTerm }
                );

                // If restaurant results are found, return only restaurant info
                if (restaurantResults.Any())
                {
                    return Ok(restaurantResults);
                }

                // Query to check if the search term matches any product name
                var productResults = _dapper.LoadDataFilterAndSearch<object>(
                    @"SELECT 
                        r.RestaurantId,
                        r.RestaurantName,
                        r.FullAddress,
                        r.Phone,
                        r.EstablishmentType,
                        p.ProductId,
                        p.ProductName,
                        p.ProductPrice
                    FROM 
                        Restaurants r
                    INNER JOIN 
                        Products p ON r.RestaurantId = p.RestaurantId
                    WHERE 
                        p.ProductName LIKE @SearchTerm",
                    new { SearchTerm }
                );

                // If product results are found, return restaurant info along with product info
                if (productResults.Any())
                {
                    return Ok(productResults);
                }

                // If no results found
                return NotFound("No results found");
            }
            catch (Exception ex)
            {
                // Log and handle the error using the ErrorHandler class
                return ErrorHandler.HandleError(ex, "Search", _logger);
            }
        }
    }
}

