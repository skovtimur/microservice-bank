using AccountService.Data;
using Microsoft.AspNetCore.Mvc;

namespace AccountService;

[ApiController]
[Route("debug")]
public class DebugController : ControllerBase
{
    [HttpGet("wallets")]
    public IActionResult GetAllWallets()
    {
        return Ok(WalletsSingleton.Wallets);
    }
    
    [HttpGet("transactions")]
    public IActionResult GetAllTransactions()
    {
        return Ok(TransactionsSingleton.Transactions);
    }
}