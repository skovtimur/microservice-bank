using AccountService.Data;
using AccountService.Domain.Entities;
using AccountService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Controllers;

[ApiController]
[Route("debug")]
public class DebugController : ControllerBase
{
    [HttpGet("wallets")]
    public IActionResult GetAllWallets()
    {
        var result = MbResult<List<WalletEntity>>.Ok(WalletsSingleton.Wallets);
        return Ok(result);
    }

    [HttpGet("transactions")]
    public IActionResult GetAllTransactions()
    {
        var result = MbResult<List<TransactionEntity>>.Ok(TransactionsSingleton.Transactions);
        return Ok(result);
    }

    [HttpGet("about-me"), Authorize]
    public IActionResult GetMe()
    {
        var dictionary = User.Claims.ToDictionary(x => x.Type, x => x.Value);
        var result = MbResult<Dictionary<string, string>>.Ok(dictionary);

        return Ok(result);
    }
}