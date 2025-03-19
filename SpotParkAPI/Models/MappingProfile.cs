using AutoMapper;
using SpotParkAPI.Models.Dtos;
using SpotParkAPI.Models.Entities;

namespace SpotParkAPI.Models
{
    public class MappingProfile:Profile
    {
        public MappingProfile()     
        {
            CreateMap<ParkingLot, ParkingLotDto>();
            CreateMap<AvailabilitySchedule, AvailabilityScheduleDto>();
        }
    }
}
