using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotParkAPI.Models;
using SpotParkAPI.Models.Entities;
using SpotParkAPI.Models.Requests;
using SpotParkAPI.Services.Interfaces;
using System.Threading.Tasks;

namespace SpotParkAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VehiclesController : ControllerBase
    {
        private readonly SpotParkDbContext _context;
        private readonly ICommonService _commonService;

        public VehiclesController(SpotParkDbContext context, ICommonService commonService)
        {
            _context = context;
            _commonService = commonService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyVehicles()
        {
            var userId = _commonService.GetCurrentUserId();

            var vehicles = await _context.UserVehicles
                .Where(v => v.UserId == userId)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();

            return Ok(vehicles);
        }

        [HttpPost]
        public async Task<IActionResult> AddVehicle([FromBody] AddVehicleRequest request)
        {
            var userId = _commonService.GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(request.PlateNumber))
                return BadRequest("Numărul de înmatriculare este obligatoriu.");

            var vehicle = new UserVehicle
            {
                UserId = userId,
                PlateNumber = request.PlateNumber.Trim().ToUpper(),
                CreatedAt = DateTime.UtcNow
            };

            _context.UserVehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            return Ok(vehicle);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var userId = _commonService.GetCurrentUserId();
            var vehicle = await _context.UserVehicles.FirstOrDefaultAsync(v => v.Id == id && v.UserId == userId);

            if (vehicle == null)
                return NotFound();

            _context.UserVehicles.Remove(vehicle);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
