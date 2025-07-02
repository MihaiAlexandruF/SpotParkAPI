using Microsoft.EntityFrameworkCore;
using SpotParkAPI.Models;
using SpotParkAPI.Models.Entities;
using SpotParkAPI.Services.Helpers;
using SpotParkAPI.Services.Interfaces;

namespace SpotParkAPI.Services
{
    public class WalletService : IWalletService
    {
        private readonly SpotParkDbContext _context;

        public WalletService(SpotParkDbContext context)
        {
            _context = context;
        }

        public async Task<Wallet> GetOrCreateWalletAsync(int userId)
        {
            var wallet = await _context.Wallets
                .Include(w => w.Transactions)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
            {
                wallet = new Wallet
                {
                    UserId = userId,
                    Balance = 0,
                    Currency = "RON",
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Wallets.Add(wallet);
                await _context.SaveChangesAsync();
            }

            return wallet;
        }

        public async Task<decimal> GetBalanceAsync(int userId)
        {
            var wallet = await GetOrCreateWalletAsync(userId);
            return wallet.Balance;
        }

        public async Task<List<WalletTransaction>> GetTransactionsAsync(int userId)
        {
            var wallet = await GetOrCreateWalletAsync(userId);
            return await _context.WalletTransactions
                .Where(t => t.WalletId == wallet.WalletId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<ServiceResult<bool>> AddTransactionAsync(int userId, decimal amount, WalletTransactionType type, string direction, string? description = null, int? reservationId = null)
        {
            var wallet = await GetOrCreateWalletAsync(userId);

            if (amount < 0)
                return ServiceResult<bool>.Fail("Suma trebuie să fie pozitivă.");

            if (direction == "out" && wallet.Balance < amount)
                return ServiceResult<bool>.Fail("Fonduri insuficiente pentru această tranzacție.");

            var transaction = new WalletTransaction
            {
                WalletId = wallet.WalletId,
                Amount = amount,
                Type = type,
                Direction = direction,
                Description = description,
                ReservationId = reservationId,
                CreatedAt = DateTime.UtcNow
            };

            wallet.Balance += direction == "in" ? amount : -amount;
            wallet.UpdatedAt = DateTime.UtcNow;

            _context.WalletTransactions.Add(transaction);
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }

        
    }
}
