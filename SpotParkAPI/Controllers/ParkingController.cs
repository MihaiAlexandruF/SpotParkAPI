using Microsoft.AspNetCore.Mvc;
using SpotParkAPI.Models.Entities;
using SpotParkAPI.Services;
using System;
using System.Collections.Generic;
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
        public async Task<ActionResult<ParkingLot>> GetParkingLotById(int id)
        {
            var spot = await _parkingService.GetParkingLotByIdAsync(id);
            if (spot == null)
            {
                return NotFound();
            }
            return Ok(spot);
        }

        [HttpPost]
        public async Task<ActionResult<ParkingLot>> AddParkingLot(ParkingLot parkingLot)
        {
            await _parkingService.AddParkingLotAsync(parkingLot);
            return CreatedAtAction(nameof(GetParkingLotById), new { id = parkingLot.ParkingLotId }, parkingLot);
        }

       
    }
}