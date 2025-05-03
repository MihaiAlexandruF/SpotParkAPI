using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotParkAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using SpotParkAPI.Models;

namespace SpotParkAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EarningsController : ControllerBase
    {
        private readonly SpotParkDbContext _context;
        private readonly ICommonService _commonService;

        public EarningsController(SpotParkDbContext context, ICommonService commonService)
        {
            _context = context;
            _commonService = commonService;
        }

        [HttpGet("per-parking")]
        public async Task<IActionResult> GetEarningsPerParking()
        {
            var userId = _commonService.GetCurrentUserId();

            var earnings = await _context.ParkingLots
                .Where(p => p.OwnerId == userId)
                .Select(p => new
                {
                    ParkingLotId = p.ParkingLotId,
                    Name = p.Description ?? p.Address,
                    Total = _context.Reservations
                        .Where(r => r.ParkingLotId == p.ParkingLotId)
                        .Sum(r => (decimal?)r.TotalCost) ?? 0
                })
                .ToListAsync();

            return Ok(earnings);
        }

        [HttpGet("total")]
        public async Task<IActionResult> GetTotalEarnings()
        {
            var userId = _commonService.GetCurrentUserId();

            var total = await _context.Reservations
                .Where(r => r.ParkingLot.OwnerId == userId)
                .SumAsync(r => (decimal?)r.TotalCost) ?? 0;

            return Ok(new { Total = total });
        }
    }
}