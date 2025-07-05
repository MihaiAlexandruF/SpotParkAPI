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
            var result = await _reservationService.ReserveParkingLotAsync(request);

            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(result.Data);
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

        [HttpGet("my-reservations/active")]
        public async Task<IActionResult> GetMyActiveReservations()
        {
            var result = await _reservationService.GetActiveReservationsAsync();
            return Ok(result);
        }

        [HttpGet("my-reservations/history")]
        public async Task<IActionResult> GetMyPastReservations()
        {
            var result = await _reservationService.GetPastReservationsAsync();
            return Ok(result);
        }


    }
}