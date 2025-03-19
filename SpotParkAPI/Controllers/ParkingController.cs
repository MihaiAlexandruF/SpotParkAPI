using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SpotParkAPI.Models.Dtos;
using SpotParkAPI.Models.Entities;
using SpotParkAPI.Models.Requests;
using SpotParkAPI.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

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

        [HttpGet("{id}")]
        public async Task<ActionResult<ParkingLotDto>> GetParkingLotById(int id)
        {
            try
            {
                var parkingLotDto = await _parkingService.GetParkingLotByIdAsync(id);
                return Ok(parkingLotDto);
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

        [HttpPost]
        public async Task<ActionResult<ParkingLot>> CreateParkingLot([FromBody] CreateParkingLotRequest request)
        {
            try
            {
                // Get the current user's ID from the JWT token
                var userId = GetCurrentUserId();
                if (userId <= 0)
                {
                    return Unauthorized("Invalid user information");
                }

                var parkingLot = await _parkingService.CreateParkingLotWithAvailabilityAsync(request, userId);
                return CreatedAtAction(nameof(GetParkingLotById), new { id = parkingLot.ParkingLotId }, parkingLot);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(500, "An error occurred while creating the parking lot");
            }
        }

        [HttpGet("{id}/availability")]
        public async Task<ActionResult<List<AvailabilitySchedule>>> GetParkingLotAvailability(int id)
        {
            try
            {
                var availability = await _parkingService.GetAvailabilitySchedulesAsync(id);
                return Ok(availability);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Parking lot not found");
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(500, "An error occurred while retrieving the availability schedules");
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                Console.WriteLine($"Invalid UserId in token: {userIdClaim.Value}");
                return userId;
            }
            Console.WriteLine("ClaimTypes.NameIdentifier not found in token.");
            return -1;
        }


    }
}