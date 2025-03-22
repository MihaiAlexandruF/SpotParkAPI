using Microsoft.AspNetCore.Mvc;
using SpotParkAPI.Models.Requests;
using SpotParkAPI.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace SpotParkAPI.Controllers
{
    [Route("api/reservations")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpPost("reserve")]
        public async Task<IActionResult> ReserveParkingLot([FromBody] ReserveRequest request)
        {
            try
            {
                var reservationDto = await _reservationService.ReserveParkingLotAsync(request);
                return Ok(reservationDto);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("check-availability")]
        public async Task<IActionResult> CheckAvailability(int parkingLotId, DateTime startTime, DateTime endTime)
        {
            try
            {
                var isAvailable = await _reservationService.IsParkingLotAvailableAsync(parkingLotId, startTime, endTime);
                return Ok(new { IsAvailable = isAvailable });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}