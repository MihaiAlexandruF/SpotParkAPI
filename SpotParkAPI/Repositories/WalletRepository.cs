using Microsoft.EntityFrameworkCore;
using SpotParkAPI.Models;
using SpotParkAPI.Models.Entities;
using SpotParkAPI.Repositories.Interfaces;
using System.Threading.Tasks;

namespace SpotParkAPI.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly SpotParkDbContext _context;

        public WalletRepository(SpotParkDbContext context)
        {
            _context = context;
        }

        public async Task<Wallet?> GetByUserIdAsync(int userId)
        {
            return await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task UpdateAsync(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();
        }
    }
}
