using SpotParkAPI.Models.Dtos;
using System;
using System.Collections.Generic;

namespace SpotParkAPI.Models.Dtos
{
    public class ParkingLotForOwnerDto
    {
        public int ParkingLotId { get; set; } // Asumăm că este int și nu Guid

        public string Description { get; set; }

        public string Address { get; set; }

        public decimal PricePerHour { get; set; }

        public double Latitude { get; set; } // trebuie să fie double pentru client (frontend)

        public double Longitude { get; set; }

        public bool IsActive { get; set; }

        public double Earnings { get; set; } // convertit din decimal cu (double)

        public List<string> ImageUrls { get; set; } = new();

        public List<AvailabilityScheduleDto> AvailabilitySchedules { get; set; } = new();
    }
}
