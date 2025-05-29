using Microsoft.AspNetCore.Mvc;
using SpotParkAPI.Models;

namespace SpotParkAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatsController : ControllerBase
    {
        private readonly SpotParkDbContext _context;

        public StatsController(SpotParkDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetStats()
        {
            var totalUsers = _context.Users.Count();
            var totalParkingLots = _context.ParkingLots.Count();

            return Ok(new
            {
                totalUsers,
                totalParkingLots
            });
        }
    }
}
