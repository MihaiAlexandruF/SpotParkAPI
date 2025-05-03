using SpotParkAPI.Models.Entities;
using SpotParkAPI.Models;
using SpotParkAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SpotParkAPI.Repositories
{
    public class ParkingImageRepository: IParkingImageRepository
    {
        private readonly SpotParkDbContext _context;

        public ParkingImageRepository(SpotParkDbContext context)
        {
            _context = context;
        }

        public async Task AddImageAsync(ParkingLotImage image)
        {
            _context.ParkingLotImages.Add(image);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetImageCountForUserAsync(int userId, int parkingLotId)
        {
            return await _context.ParkingLotImages
                .CountAsync(img => img.UserId == userId && img.ParkingLotId == parkingLotId);
        }

        public async Task<List<ParkingLotImage>> GetImagesForParkingLotAsync(int parkingLotId)
        {
            return await _context.ParkingLotImages
                .Where(img => img.ParkingLotId == parkingLotId)
                .ToListAsync();
        }
    }
}
