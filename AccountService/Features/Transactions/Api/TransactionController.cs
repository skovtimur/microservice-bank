using System.ComponentModel.DataAnnotations;
using System.Net;
using AccountService.Features.Transactions.Api.Requests;
using AccountService.Features.Transactions.CreateTransaction;
using AccountService.Features.Transactions.Domain;
using AccountService.Features.Transactions.GetAllTransactions;
using AccountService.Features.Transactions.GetTransaction;
using AccountService.Shared.Abstractions.ServiceInterfaces;
using AccountService.Shared.Api.Filters;
using AccountService.Shared.Domain;
using AccountService.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Features.Transactions.Api;

[ApiController]
[Route("/api/transactions")]
public class TransactionController(
    IMediator mediator,
    IClaimsService claimsService,
    ILogger<TransactionController> logger) : ControllerBase
{
    // ReSharper disable NullableWarningSuppressionIsUsed
    //Чтобы не блокировали Оператор ! (null-forgiving operator), а точнее я использую MbResult где Result может быть null ТОЛЬКО ПРИ isSuccess = false,
    //я же делаю проверку поэтому данный warning излишен

    /// <summary>
    /// Get a transaction by id
    /// </summary>
    /// <param name="id">Transaction Id as Guid</param>
    /// <returns> Found transaction </returns>
    /// <response code="200">Returns the found transaction</response>
    /// <response code="401">If the user haven't logged in in the system</response>
    /// <response code="403">If the user want to see his not transactions</response>
    /// <response code="404">If the transaction doesn't exist</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id:guid}", Name = nameof(GetTransaction)), Authorize]
    public async Task<IActionResult> GetTransaction(Guid id)
    {
        if (claimsService.TryGetUserId(User, out var userId) == false)
            return Unauthorized(MbResult<TransactionDto>.Fail("You haven't entered in the system"));

        try
        {
            var transaction = await mediator.Send(new GetTransactionQuery(id, userId));

            if (transaction == null)
                return NotFound(MbResult<TransactionDto>.Fail("Transaction not found"));

            return Ok(MbResult<TransactionDto>.Ok(transaction));
        }
        catch (ForbiddenException exception)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                exception.ToMbResult<TransactionDto>());
        }
        catch (NotFoundException exception)
        {
            return NotFound(exception.ToMbResult<TransactionDto>());
        }
    }

    /// <summary>
    /// Get a transaction by Account ID
    /// </summary>
    /// <param name="accountId">Account ID as Guid</param>
    /// <param name="fromAtUtc">Filtering starts form this DateTime(UTC)</param>
    /// <returns> List of transaction </returns>
    /// <response code="200">Returns the found transactions</response>
    /// <response code="401">If the user haven't logged in in the system</response>
    /// <response code="404">If the account wasn't found</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("all/{accountId:guid}/{fromAtUtc:datetime}"), Authorize]
    public async Task<IActionResult> GetAnExtract([Required] Guid accountId, [Required] DateTime fromAtUtc)
    {
        if (claimsService.TryGetUserId(User, out var userId) == false)
            return Unauthorized(MbResult<List<TransactionDto>>.Fail("You haven't entered in the system"));

        try
        {
            var transactions =
                await mediator.Send(new GetAllTransactionsQuery(accountId, userId, fromAtUtc));

            var result = MbResult<List<TransactionDto>>.Ok(transactions);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.ToMbResult<List<TransactionDto>>());
        }
        catch (ForbiddenException exception)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                exception.ToMbResult<List<TransactionDto>>());
        }
    }

    /// <summary>
    /// Creates a transaction
    /// </summary>
    /// <returns> ID of the new transaction </returns>
    /// <response code="200">Creates the transaction</response>
    /// <response code="400">Whether the Description is empty or more than 5.000 characters
    /// OR the Currency Code doesn't comply with ISO 4217 format
    /// OR a User take invalid data
    /// OR the account or the counterparty account has already been deleted</response>
    /// <response code="401">If the user haven't logged in in the system</response>
    /// <response code="402">Balance of Account or Counterparty Account is less than the sum of transaction</response>
    /// <response code="403">If a User try to take someone else's money OR the User isn't the owner of it OR the User's accounts were frozen</response>
    /// <response code="404">If the account or the counterparty account wasn't found</response>
    /// <response code="409">If there was a conflict while updating the database</response> 
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost("create"), ValidationFilter, Authorize]
    public async Task<IActionResult> CreateTransaction([Required, FromForm] TransactionCreateRequest request)
    {
        if (request.AccountId == request.CounterpartyAccountId)
            return BadRequest(MbResult<Guid>.Fail("AccountId and CounterpartyAccountId are same"));

        if (claimsService.TryGetUserId(User, out var userId) == false)
            return Unauthorized(MbResult<List<TransactionDto>>.Fail("You haven't entered in the system"));

        try
        {
            var descriptionResult = DescriptionValueObject.Create(request.Description);

            if (descriptionResult.IsSuccess == false)
                return BadRequest(MbResult<Guid>.Fail(descriptionResult.Error!));

            var currency = CurrencyValueObject.Create(request.IsoCurrencyCode);

            if (currency == null)
                return BadRequest(MbResult<Guid>.Fail($"The Currency code({request.IsoCurrencyCode}) isn't allowed"));

            var result = TransactionCreateCommand.Create(userId, request.AccountId,
                request.Sum, request.TransactionType,
                currency, descriptionResult.Result!, request.CounterpartyAccountId);

            if (result.IsSuccess == false)
                return BadRequest(MbResult<Guid>.Fail(result.Error!));

            var transactionId = await mediator.Send(result.Result!);
            return CreatedAtAction(nameof(GetTransaction), new { id = transactionId }, transactionId);
        }
        catch (BadRequestException exception)
        {
            return BadRequest(exception.ToMbResult<Guid>());
        }
        catch (NotFoundException exception)
        {
            return NotFound(exception.ToMbResult<Guid>());
        }
        catch (ForbiddenException exception)
        {
            return StatusCode(StatusCodes.Status403Forbidden, exception.ToMbResult<Guid>());
        }
        catch (PaymentRequiredException exception)
        {
            return StatusCode((int)HttpStatusCode.PaymentRequired,
                new { message = exception.ToMbResult<Guid>() });
        }
        catch (DbUpdateConcurrencyException ex)
        {
            logger.LogError(ex, "Database update failed.");
            return Conflict(MbResult<Guid>.Fail(ex.Message));
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database update failed.");
            return Conflict(MbResult<Guid>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Transfers the money(creates new transaction)
    /// </summary>
    /// <returns> ID of the new transaction </returns>
    /// <response code="200">Creates the transaction</response>
    /// <response code="400">Whether the Description is empty or more than 5.000 characters
    /// OR the Currency Code doesn't comply with ISO 4217 format
    /// OR a User take invalid data
    /// OR the account or the counterparty account has already been deleted</response>
    /// <response code="401">If the user haven't logged in in the system</response>
    /// <response code="402">Balance of Account or Counterparty Account is less than the sum of transaction</response>
    /// <response code="403">Whether a User try to take someone else's money OR a User isn't the owner of it</response>
    /// <response code="404">If the account or the counterparty account wasn't found</response>
    /// /// <response code="409">If there was a conflict while updating the database</response> 
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost("transfer-money"), ValidationFilter, Authorize]
    public async Task<IActionResult> TransferMoney([Required, FromForm] TransferMoneyRequest request)
    {
        var transactionCreateRequest = new TransactionCreateRequest
        {
            AccountId = request.AccountId,
            CounterpartyAccountId = request.TransferToCounterpartyAccountId,
            Sum = request.Sum,
            TransactionType = TransactionType.Debit,
            IsoCurrencyCode = request.IsoCurrencyCode,
            Description = request.Description
        };
        return await CreateTransaction(transactionCreateRequest);
    }
}