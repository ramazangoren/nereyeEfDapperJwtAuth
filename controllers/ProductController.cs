using System;
using System.Collections.Generic;
using api.Data;
using api.DTOs.ProductDto;
using api.Helpers;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly NereyeDBContext _dapper;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IConfiguration config, ILogger<ProductController> logger)
        {
            _dapper = new NereyeDBContext(config);
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("AllProducts/{RestaurantId:int}")]
        public ActionResult<IEnumerable<Product>> GetProducts(int RestaurantId)
        {
            try
            {
                string sql = "SELECT * FROM Products WHERE RestaurantId = " + RestaurantId;
                var products = _dapper.LoadData<Product>(sql);

                return products.Any()
                    ? Ok(products)
                    : NotFound($"No products found for RestaurantId: {RestaurantId}");
            }
            catch (Exception ex)
            {
                return ErrorHandler.HandleError(ex, "GetProducts", _logger);
            }
        }

        [AllowAnonymous]
        [HttpGet("{RestaurantId:int}/{ProductId:int}")]
        public ActionResult<Product> GetProduct(int RestaurantId, int ProductId)
        {
            try
            {
                var product = _dapper.LoadDataSingle<Product>(
                    "SELECT * FROM Products WHERE RestaurantId = @RestaurantId AND ProductId = @ProductId",
                    new { RestaurantId, ProductId }
                );
                return product == null ? NotFound("No products found") : Ok(product);
            }
            catch (Exception ex)
            {
                return ErrorHandler.HandleError(ex, "GetProduct", _logger);
            }
        }

        // [AllowAnonymous]
        [HttpPost("AddProduct/{RestaurantId:int}")]
        public ActionResult AddProduct(AddProductDto productDto, int RestaurantId)
        {
            try
            {
                string UserId = User.FindFirst("userId")?.Value + "";
                if (productDto == null)
                    return BadRequest("Product data is required.");

                var restaurantOwnershipQuery =
                    "SELECT COUNT(1) FROM Restaurants WHERE RestaurantId = @RestaurantId AND UserId = @UserId";
                int restaurantOwnedByUser = _dapper.ExecuteSqlWithRowCount(
                    restaurantOwnershipQuery,
                    new { RestaurantId, UserId }
                );
                if (restaurantOwnedByUser == 0)
                {
                    return NotFound(
                        "Restaurant does not exist or user does not own the restaurant."
                    );
                }
                var query =
                    @"INSERT INTO dbo.Products (
                RestaurantId, ProductName, ProductPhoto, Category, ProductPrice, ProductExplanation, ProductPreparationTime, CreatedAt
            ) VALUES (
                @RestaurantId, @ProductName, @ProductPhoto, @Category, @ProductPrice, @ProductExplanation, @ProductPreparationTime, @CreatedAt
            );";

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

                bool productCreated = _dapper.ExecuteSql(query, parameters);

                return productCreated
                    ? Ok("Product added successfully.")
                    : BadRequest("Could not add the product.");
            }
            catch (Exception ex)
            {
                return ErrorHandler.HandleError(ex, "AddProduct", _logger);
            }
        }

        [HttpPut("UpdateProduct/{RestaurantId:int}/{ProductId:int}")]
        public ActionResult UpdateProduct(
            UpdateProductDto updateProductDto,
            int RestaurantId,
            int ProductId
        )
        {
            try
            {
                string UserId = User.FindFirst("userId")?.Value + "";
                if (updateProductDto == null)
                    return BadRequest("Product data is required.");

                var restaurantOwnershipQuery =
                    "SELECT COUNT(1) FROM Restaurants WHERE RestaurantId = @RestaurantId AND UserId = @UserId";
                int restaurantOwnedByUser = _dapper.ExecuteSqlWithRowCount(
                    restaurantOwnershipQuery,
                    new { RestaurantId, UserId }
                );

                if (restaurantOwnedByUser == 0)
                {
                    return NotFound(
                        "Restaurant does not exist or user does not own the restaurant."
                    );
                }
                var productExistsQuery =
                    "SELECT COUNT(1) FROM Products WHERE ProductId = @ProductId AND RestaurantId = @RestaurantId";
                int productExists = _dapper.ExecuteSqlWithRowCount(
                    productExistsQuery,
                    new { ProductId, RestaurantId }
                );

                if (productExists == 0)
                {
                    return NotFound(
                        $"Product with ID {ProductId} not found for Restaurant ID {RestaurantId}."
                    );
                }

                var updateQuery =
                    @"UPDATE Products 
              SET ProductName = @ProductName, ProductPhoto = @ProductPhoto, Category = @Category, 
                  ProductPrice = @ProductPrice, ProductExplanation = @ProductExplanation, 
                  ProductPreparationTime = @ProductPreparationTime, UpdatedAt = @UpdatedAt
              WHERE ProductId = @ProductId AND RestaurantId = @RestaurantId";

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
                    UpdatedAt = DateTime.Now,
                };

                bool result = _dapper.ExecuteSql(updateQuery, parameters);

                return result
                    ? Ok("Product updated successfully.")
                    : BadRequest("Could not update the product.");
            }
            catch (Exception ex)
            {
                return ErrorHandler.HandleError(ex, "UpdateProduct", _logger);
            }
        }

        [HttpDelete("DeleteProduct/{RestaurantId:int}/{ProductId:int}")]
        public ActionResult DeleteProduct(int RestaurantId, int ProductId)
        {
            try
            {
                string UserId = User.FindFirst("userId")?.Value + "";

                var restaurantOwnershipQuery =
                    "SELECT COUNT(1) FROM Restaurants WHERE RestaurantId = @RestaurantId AND UserId = @UserId";
                int restaurantOwnedByUser = _dapper.ExecuteSqlWithRowCount(
                    restaurantOwnershipQuery,
                    new { RestaurantId, UserId }
                );

                if (restaurantOwnedByUser == 0)
                {
                    return NotFound(
                        "Restaurant does not exist or user does not own the restaurant."
                    );
                }

                var productExistsQuery =
                    "SELECT COUNT(1) FROM Products WHERE ProductId = @ProductId AND RestaurantId = @RestaurantId";
                int productExists = _dapper.ExecuteSqlWithRowCount(
                    productExistsQuery,
                    new { ProductId, RestaurantId }
                );

                if (productExists == 0)
                {
                    return NotFound(
                        $"Product with ID {ProductId} not found for Restaurant ID {RestaurantId}."
                    );
                }
                var deleteQuery = "DELETE FROM Products WHERE ProductId = @ProductId";
                bool result = _dapper.ExecuteSql(deleteQuery, new { ProductId });

                return result
                    ? Ok("Product has been deleted.")
                    : BadRequest("Product could not be deleted.");
            }
            catch (Exception ex)
            {
                return ErrorHandler.HandleError(ex, "DeleteProduct", _logger);
            }
        }
    }
}
