using Microsoft.AspNetCore.Mvc;
using SpotParkAPI.Models.Requests;
using SpotParkAPI.Services;
using System;
using System.Threading.Tasks;

namespace SpotParkAPI.Controllers
{
    [Route("api/parking/{parkingLotId}/availability")]
    [ApiController]
    public class AvailabilityController : ControllerBase
    {
        private readonly IAvailabilityService _availabilityService;

        public AvailabilityController(IAvailabilityService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailabilitySchedules(int parkingLotId)
        {
            try
            {
                var schedules = await _availabilityService.GetAvailabilitySchedulesAsync(parkingLotId);
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving the availability schedules");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAvailability(int parkingLotId, [FromBody] AvailabilitySchedulesRequest request)
        {
            try
            {
                await _availabilityService.UpdateAvailabilityAsync(parkingLotId, request);
                return Ok("Availability updated successfully");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the availability");
            }
        }
    }
}