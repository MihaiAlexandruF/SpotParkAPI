
namespace SpotParkAPI.Models.Entities
{
    public class Wallet
    {
        public int WalletId { get; set; }
        public int UserId { get; set; }
        public decimal Balance { get; set; } = 0;
        public string Currency { get; set; } = "RON";
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; } = null!;
        public virtual ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
    }
}
