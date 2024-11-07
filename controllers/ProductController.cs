using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.ProductDto;
using api.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace api.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IDbConnection _dbConnection;

        public ProductController(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        [HttpGet("AllRestaurants/{RestaurantId:int}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(int RestaurantId)
        {
            var products = await _dbConnection.QueryAsync<Product>(
                "SELECT * FROM Products WHERE RestaurantId = @RestaurantId",
                new { RestaurantId }
            );

            return products.Any()
                ? Ok(products)
                : NotFound($"No products found for RestaurantId: {RestaurantId}");
        }

        [HttpGet("Restaurant/{RestaurantId:int}/{ProductId:int}")]
        public async Task<ActionResult<Product>> GetProduct(int RestaurantId, int ProductId)
        {
            var product = await _dbConnection.QueryFirstOrDefaultAsync<Product>(
                "SELECT * FROM Products WHERE RestaurantId = @RestaurantId AND ProductId = @ProductId",
                new { RestaurantId, ProductId }
            );
            return product == null ? NotFound("No products found") : Ok(product);
        }

        [HttpPost("AddProduct/{RestaurantId:int}")]
        public async Task<ActionResult<AddProductDto>> AddProduct(
            AddProductDto productDto,
            int RestaurantId
        )
        {
            if (productDto == null) return BadRequest("Product data is required.");

            var restaurantExists = await _dbConnection.QueryFirstOrDefaultAsync<int>(
                "SELECT COUNT(1) FROM Restaurants WHERE RestaurantId = @RestaurantId",
                new { RestaurantId }
            );

            if (restaurantExists == 0) return NotFound($"Restaurant with ID {RestaurantId} not found.");

            var query =
                @"INSERT INTO Products (
                            RestaurantId, ProductName, ProductPhoto, Category, ProductPrice, 
                            ProductExplanation, ProductPreparationTime, CreatedAt
                        ) VALUES (
                            @RestaurantId, @ProductName, @ProductPhoto, @Category, 
                            @ProductPrice, @ProductExplanation, @ProductPreparationTime, @CreatedAt
                        );
                        SELECT CAST(SCOPE_IDENTITY() AS int);";

            // SELECT CAST(SCOPE_IDENTITY() AS int) = The SQL statement SELECT CAST(SCOPE_IDENTITY() AS int) is used in
            // the context of an INSERT operation in SQL Server to retrieve the last identity value that was generated
            // for an identity column in the current session and the current scope. Here’s a// breakdown of the components;

            var parameters = new
            {
                RestaurantId,
                productDto.ProductName,
                productDto.ProductPhoto,
                productDto.Category,
                productDto.ProductPrice,
                productDto.ProductExplanation,
                productDto.ProductPreparationTime,
                CreatedAt = DateTime.Now,
            };

            // Insert product and get the new ProductId
            // var sth = await _dbConnection.QuerySingleAsync<int>(query, parameters); this returns the id of the product
            // var sth = await _dbConnection.ExecuteAsync(query, parameters); this returns 1 which means true which means product was created
            // await _dbConnection.ExecuteAsync(query, parameters); this doesnt return anything but execute the code
            await _dbConnection.ExecuteAsync(query, parameters);

            return Ok(productDto);
        }

        [HttpPut("UpdateProduct/{RestaurantId:int}/{ProductId:int}")]
        public async Task<ActionResult<UpdateProductDto>> UpdateProduct(
            UpdateProductDto updateProductDto,
            int RestaurantId,
            int ProductId
        )
        {
            if (updateProductDto == null)
                return BadRequest("Product data is required.");

            var restaurantExists = await _dbConnection.QueryFirstOrDefaultAsync<int>(
                "SELECT COUNT(1) FROM Restaurants WHERE RestaurantId = @RestaurantId",
                new { RestaurantId }
            );

            if (restaurantExists == 0)
                return NotFound($"Restaurant with ID {RestaurantId} not found.");

            var productExists = await _dbConnection.QueryFirstOrDefaultAsync<int>(
                "SELECT COUNT(1) FROM Products WHERE ProductId = @ProductId AND RestaurantId = @RestaurantId",
                new { ProductId, RestaurantId }
            );

            if (productExists == 0)
                return NotFound(
                    $"Product with ID {ProductId} not found for Restaurant ID {RestaurantId}."
                );

            var query =
                @"
                UPDATE Products 
                SET 
                    ProductName = @ProductName, ProductPhoto = @ProductPhoto, Category = @Category, 
                    ProductPrice = @ProductPrice, ProductExplanation = @ProductExplanation, 
                    ProductPreparationTime = @ProductPreparationTime, UpdatedAt = @UpdatedAt
                WHERE 
                    ProductId = @ProductId AND RestaurantId = @RestaurantId";

            var UpdatedAt = DateTime.Now;

            var parameters = new
            {
                RestaurantId,
                ProductId,
                updateProductDto.ProductName,
                updateProductDto.ProductPhoto,
                updateProductDto.Category,
                updateProductDto.ProductPrice,
                updateProductDto.ProductExplanation,
                updateProductDto.ProductPreparationTime,
                UpdatedAt,
            };

            // Execute the update query
            await _dbConnection.ExecuteAsync(query, parameters);

            return Ok(updateProductDto);
        }

        [HttpDelete("DeleteProduct/{RestaurantId:int}/{ProductId:int}")]
        public async Task<ActionResult> DeleteProduct(int RestaurantId, int ProductId)
        {
            var restaurantExists = await _dbConnection.QueryFirstOrDefaultAsync<int>(
                "SELECT COUNT(1) FROM Restaurants WHERE RestaurantId = @RestaurantId",
                new { RestaurantId }
            );

            if (restaurantExists == 0)
                return NotFound($"Restaurant with ID {RestaurantId} not found.");

            // Check if the ProductId exists and belongs to the specified RestaurantId
            var productExists = await _dbConnection.QueryFirstOrDefaultAsync<int>(
                "SELECT COUNT(1) FROM Products WHERE ProductId = @ProductId AND RestaurantId = @RestaurantId",
                new { ProductId, RestaurantId }
            );

            if (productExists == 0)
                return NotFound(
                    $"Product with ID {ProductId} not found for Restaurant ID {RestaurantId}."
                );

            var deleteQuery = "DELETE FROM Products WHERE ProductId = " + ProductId;
            var rowsAffected = await _dbConnection.ExecuteAsync(deleteQuery);

            return rowsAffected > 0 ? Ok("product been deleted") : NotFound();
        }
    }
}
