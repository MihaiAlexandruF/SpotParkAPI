using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotParkAPI.Services;
using SpotParkAPI.Services.Interfaces;
namespace SpotParkAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly ICommonService _commonService;

        public WalletController(IWalletService walletService, ICommonService commonService)
        {
            _walletService = walletService;
            _commonService = commonService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBalance()
        {
            var userId = _commonService.GetCurrentUserId();
            var balance = await _walletService.GetBalanceAsync(userId);
            return Ok(new { balance, currency = "RON" });
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions()
        {
            var userId = _commonService.GetCurrentUserId();
            var transactions = await _walletService.GetTransactionsAsync(userId);
            return Ok(transactions);
        }
    }
}
