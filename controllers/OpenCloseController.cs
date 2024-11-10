using System.Threading.Tasks;
using api.DTOs.RestaurantDto;
using api.Helpers;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OpenCloseController : ControllerBase
    {
        private readonly IOpenCloseService _openCloseService;
        private readonly ILogger<OpenCloseController> _logger;

        public OpenCloseController(
            IOpenCloseService openCloseService,
            ILogger<OpenCloseController> logger
        )
        {
            _openCloseService = openCloseService;
            _logger = logger;
        }

        [HttpGet("CheckIfOpenOrClosed/{restaurantId}")]
        public ActionResult<string> GetOpenStatus(int restaurantId)
        {
            try
            {
                // Use the service to check if the restaurant is open or closed
                var status = _openCloseService.CheckIfOpenOrClosed(restaurantId);
                // if (status == true)
                // {
                //     return Ok("open");
                // }
                // else
                // {
                //     return Ok("closed");
                // }

                return Ok(status);
            }
            catch (Exception ex)
            {
                // Handle the error using the ErrorHandler class
                return ErrorHandler.HandleError(ex, "GetOpenStatus", _logger);
            }
        }
    }
}
