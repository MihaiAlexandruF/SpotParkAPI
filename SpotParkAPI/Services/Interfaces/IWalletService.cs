using SpotParkAPI.Models.Entities;
using System.Threading.Tasks;

namespace SpotParkAPI.Services.Interfaces
{
    public interface IWalletService
    {
        Task<Wallet> GetOrCreateWalletAsync(int userId);

        Task<decimal> GetBalanceAsync(int userId);

        Task<List<WalletTransaction>> GetTransactionsAsync(int userId);

    }
}
