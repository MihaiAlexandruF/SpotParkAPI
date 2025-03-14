using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotParkAPI.Models;

namespace SpotParkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingController : ControllerBase
    {
        private readonly SpotParkDbContext _context;

        public ParkingController(SpotParkDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParkingLot>>> GetParkingLots()
        {
            return  await _context.ParkingLots.ToListAsync();
        }





    }
    
        
        



    
}
