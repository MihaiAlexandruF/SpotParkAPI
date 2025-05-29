using Microsoft.AspNetCore.Mvc;
using SpotParkAPI.Models.Requests;
using SpotParkAPI.Services;
using SpotParkAPI.Services.Helpers;
using SpotParkAPI.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace SpotParkAPI.Controllers
{
    [Route("api/reservations")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly ICommonService _commonService;
        private readonly IReservationService _reservationService;

        public ReservationController(ICommonService commonService,IReservationService reservationService)
        {
            _commonService = commonService;
            _reservationService = reservationService;
        }

        [HttpPost("reserve")]
        public async Task<IActionResult> ReserveParkingLot([FromBody] CreateReservationRequest request)
        {
            if (request.StartTime >= request.EndTime)
            {
                return BadRequest("Data de sfârșit trebuie să fie după data de început.");
            }

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
            catch (Exception)
            {
                return StatusCode(500, "A apărut o eroare internă.");
            }
        }


        [HttpGet("check-availability")]
        public async Task<IActionResult> CheckAvailability(int parkingLotId, DateTime startTime, DateTime endTime)
        {
            var utcStartTime = TimeZoneService.ConvertLocalToUtc(startTime);
            var utcEndTime = TimeZoneService.ConvertLocalToUtc(endTime);

            var isAvailable = await _reservationService.IsParkingLotAvailableAsync(parkingLotId, utcStartTime, utcEndTime);
            return Ok(new { IsAvailable = isAvailable });
        }


        [HttpGet("active-clients")]
        public async Task<IActionResult> GetActiveClients()
        {
            var userId = _commonService.GetCurrentUserId();
            var clients = await _reservationService.GetActiveClientsAsync(userId);
            return Ok(clients);
        }

    }
}