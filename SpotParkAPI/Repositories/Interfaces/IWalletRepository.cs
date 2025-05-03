using SpotParkAPI.Models.Entities;
using System.Threading.Tasks;

namespace SpotParkAPI.Repositories.Interfaces
{
    public interface IWalletRepository
    {
        Task<Wallet?> GetByUserIdAsync(int userId);
        Task UpdateAsync(Wallet wallet);
    }
}
