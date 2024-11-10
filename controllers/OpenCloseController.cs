using System.Threading.Tasks;
using api.DTOs.RestaurantDto;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OpenCloseController : ControllerBase
    {
        private readonly IOpenCloseService _openCloseService;

        public OpenCloseController(IOpenCloseService openCloseService)
        {
            _openCloseService = openCloseService;
        }

        [HttpGet("CheckIfOpenOrClosed/{restaurantId}")]
        public async Task<ActionResult<string>> GetOpenStatus(int restaurantId)
        {
            // Use the service to check if the restaurant is open or closed
            var status = await _openCloseService.CheckIfOpenOrClosed(restaurantId);
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
    }
}
