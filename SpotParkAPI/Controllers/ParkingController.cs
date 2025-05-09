﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotParkAPI.Models.Dtos;
using SpotParkAPI.Models.Entities;
using SpotParkAPI.Models.Requests;
using SpotParkAPI.Repositories.Interfaces;
using SpotParkAPI.Services;
using SpotParkAPI.Services.Interfaces;
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
        private readonly IParkingRepository _parkingRepository;
        private readonly ICommonService _commonService;

        public ParkingController(ParkingService parkingService,ICommonService commonService, IParkingRepository parkingRepository)
        {
            _parkingService = parkingService;
            _commonService = commonService;
            _parkingRepository = parkingRepository;
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
            
                var parkingLotDto = await _parkingService.GetParkingLotByIdAsync(id);
                return Ok(parkingLotDto);              
        }

        [HttpPost]
        public async Task<ActionResult<ParkingLot>> CreateParkingLot([FromBody] CreateParkingLotRequest request)
        {
            try
            {
                // Get the current user's ID from the JWT token
                var userId = _commonService.GetCurrentUserId();
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

        [HttpGet("my-spots")]
        [Authorize]
        public async Task<ActionResult<List<ParkingLotForOwnerDto>>> GetMySpots()
        {
            var userId = _commonService.GetCurrentUserId();

            var parkingLots = await _parkingService.GetParkingLotsForOwnerDashboardAsync(userId);

            return Ok(parkingLots);
        }


        [HttpGet("{id}/details")]
        public async Task<ActionResult<ParkingLotDto>> GetParkingLotDetails(int id)
        {
            var parkingLotDetails = await _parkingService.GetParkingLotDetailsByIdAsync(id);
            return Ok(parkingLotDetails);
        }


        [HttpPut("{parkingLotId}/toggle-active")]
        [Authorize]
        public async Task<IActionResult> ToggleActive(int parkingLotId)
        {
            var userId = _commonService.GetCurrentUserId();
            try
            {
                var newStatus = await _parkingService.ToggleParkingLotActiveStatusAsync(userId, parkingLotId);
                return Ok(new { ParkingLotId = parkingLotId, IsActive = newStatus });
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Locul de parcare nu există.");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("Nu ești proprietarul acestui loc de parcare.");
            }
        }



    }
}