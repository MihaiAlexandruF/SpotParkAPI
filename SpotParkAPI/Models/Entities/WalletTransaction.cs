using System;

namespace SpotParkAPI.Models.Entities
{
    public class WalletTransaction
    {
        public int WalletTransactionId { get; set; }
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public WalletTransactionType Type { get; set; }
        public string Direction { get; set; } = "in"; // "in" = credit, "out" = debit
        public int? ReservationId { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Wallet Wallet { get; set; } = null!;
        public virtual Reservation? Reservation { get; set; }
    }

    public enum WalletTransactionType
    {
        Topup,
        ReservationPayment,
        Earning,
        Commission,
        Withdrawal
    }
}
