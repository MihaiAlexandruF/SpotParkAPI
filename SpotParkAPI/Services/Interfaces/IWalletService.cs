using SpotParkAPI.Models.Entities;
using SpotParkAPI.Services.Helpers;
using System.Threading.Tasks;

namespace SpotParkAPI.Services.Interfaces
{
    public interface IWalletService
    {
        Task<Wallet> GetOrCreateWalletAsync(int userId);

        Task<decimal> GetBalanceAsync(int userId);

        Task<List<WalletTransaction>> GetTransactionsAsync(int userId);

        Task<ServiceResult<bool>> AddTransactionAsync(
    int userId,
    decimal amount,
    WalletTransactionType type,
    string direction,
    string? description = null,
    int? reservationId = null
);



    }
}
