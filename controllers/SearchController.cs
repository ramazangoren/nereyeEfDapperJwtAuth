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
    public class SearchController : ControllerBase
    {
        private readonly IDbConnection _dbConnection;

        public SearchController(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<object>>> Search([FromQuery] string searchParam)
        {
            if (string.IsNullOrWhiteSpace(searchParam)) return BadRequest("Search parameter is required.");

            string searchTerm = $"%{searchParam}%";

            var restaurantResults = await _dbConnection.QueryAsync(
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
                            new { SearchTerm = searchTerm }
                        );

            // If restaurant results are found, return only restaurant info
            if (restaurantResults.Any())
            {
                return Ok(restaurantResults);
            }

            // Query to check if the search term matches any product name
            var productResults = await _dbConnection.QueryAsync(
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
                        new { SearchTerm = searchTerm }
                    );

            // If product results are found, return restaurant info along with product info
            if (productResults.Any())
            {
                return Ok(productResults);
            }

            // If no results found
            return NotFound("No results found");
        }
    
    
    }
}
