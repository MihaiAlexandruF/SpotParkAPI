using Microsoft.EntityFrameworkCore;
using SpotParkAPI.Models;

namespace SpotParkAPI.Repositories
{
    public class ParkingRepository : IParkingRepository
    {
        private readonly SpotParkDbContext _context;

        public ParkingRepository(SpotParkDbContext context)
        {
            _context = context;
        }


        public async Task<List<ParkingLot>> GetParkingLotsAsync()
        {
            return await _context.ParkingLots.ToListAsync();
            //de facyt pt ID
        }
    }
}
