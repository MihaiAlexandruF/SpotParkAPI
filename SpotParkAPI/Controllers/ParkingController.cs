using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotParkAPI.Models;
using SpotParkAPI.Services;

namespace SpotParkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingController : ControllerBase
    {

        private readonly ParkingService _parkingService;

        public ParkingController(ParkingService parkingService)
        {
            _parkingService = parkingService;
        }
        [HttpGet]
        public async Task<ActionResult<List<ParkingLot>>> GetParkingLots()
        {
            var spots = await _parkingService.GetParkingLotsAsync();
            return Ok(spots);
        }



    }
    
        
        



    
}
