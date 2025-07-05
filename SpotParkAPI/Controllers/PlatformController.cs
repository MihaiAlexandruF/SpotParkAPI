using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotParkAPI.Models;
using SpotParkAPI.Models.Entities;
using System.Globalization;

namespace SpotParkAPI.Controllers.Platform
{
    [ApiController]
    [Route("api/platform")]
    [Authorize(Roles = "Platform")]
    public class PlatformController : ControllerBase
    {
        private readonly SpotParkDbContext _context;

        public PlatformController(SpotParkDbContext context)
        {
            _context = context;
        }

        [HttpGet("earnings")]
        public async Task<IActionResult> GetPlatformEarnings()
        {
            var commissionTransactions = await _context.WalletTransactions
                .Include(t => t.Wallet)
                .ThenInclude(w => w.User)
                .Where(t => t.Type == WalletTransactionType.Commission && t.Direction == "in")
                .ToListAsync();

            var totalEarnings = commissionTransactions.Sum(t => t.Amount);
            var numberOfTransactions = commissionTransactions.Count;

            var last7Days = commissionTransactions
                .Where(t => t.CreatedAt >= DateTime.UtcNow.AddDays(-6))
                .GroupBy(t => t.CreatedAt.Date)
                .Select(g => new
                {
                    date = g.Key.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    amount = g.Sum(t => t.Amount)
                })
                .OrderBy(x => x.date)
                .ToList();

            var topUsers = commissionTransactions
                .GroupBy(t => t.Wallet.UserId)
                .Select(g => new
                {
                    ownerId = g.Key,
                    username = g.First().Wallet.User.Username,
                    commissionGenerated = g.Sum(t => t.Amount)
                })
                .OrderByDescending(x => x.commissionGenerated)
                .Take(5)
                .ToList();

            return Ok(new
            {
                totalEarnings,
                numberOfTransactions,
                last7DaysBreakdown = last7Days,
                topUsers
            });
        }
    }
}
