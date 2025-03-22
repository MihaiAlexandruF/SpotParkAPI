using AutoMapper;
using SpotParkAPI.Models.Dtos;
using SpotParkAPI.Models.Entities;
using SpotParkAPI.Models.Requests;

namespace SpotParkAPI.Models
{
    public class MappingProfile:Profile
    {
        public MappingProfile()     
        {
            CreateMap<ParkingLot, ParkingLotDto>();
            CreateMap<AvailabilitySchedule, AvailabilityScheduleDto>();
            CreateMap<ReserveRequest, Reservation>()
            .ForMember(dest => dest.ReservationId, opt => opt.Ignore()) // ID-ul e generat de DB
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "active")); // Setează statusul implicit
            CreateMap<Reservation, ReservationDto>();
        }
    }
}
