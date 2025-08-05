using System.ComponentModel.DataAnnotations;
using System.Net;
using AccountService.Abstractions.ServiceInterfaces;
using AccountService.Commands.CreateTransaction;
using AccountService.Domain;
using AccountService.Domain.Entities;
using AccountService.Domain.ValueObjects;
using AccountService.DTOs;
using AccountService.Exceptions;
using AccountService.Extensions;
using AccountService.Filters;
using AccountService.Queries.GetAllTransactions;
using AccountService.Queries.GetTransaction;
using AccountService.Requests;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Controllers;

[ApiController]
[Route("transactions")]
public class TransactionController(
    IMapper mapper,
    IMediator mediator,
    ILogger<TransactionController> logger,
    IClaimsService claimsService) : ControllerBase
{
    /// <summary>
    /// Get a transaction by id
    /// </summary>
    /// <param name="id">Transaction Id as Guid</param>
    /// <returns> Found transaction </returns>
    /// <response code="200">Returns the found transaction</response>
    /// <response code="404">If the transaction doesn't exist</response>
    /// <response code="401">If the user haven't logined in the system</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("{id:guid}", Name = nameof(GetTransaction)), Authorize]
    public async Task<IActionResult> GetTransaction(Guid id)
    {
        if (claimsService.TryGetUserId(User, out var userId) == false)
            return Unauthorized(MbResult<TransactionDto>.Fail("You haven't entered in the system"));

        try
        {
            TransactionEntity? transaction = await mediator.Send(new GetTransactionQuery(id, userId));

            if (transaction == null)
                return NotFound(MbResult<TransactionDto>.Fail("Transaction not found"));

            var dto = mapper.Map<TransactionDto>(transaction);
            return Ok(MbResult<TransactionDto>.Ok(dto));
        }
        catch (ForbiddenException exception)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                exception.ToMbResult<TransactionDto>());
        }
    }

    /// <summary>
    /// Get a transaction by Account ID
    /// </summary>
    /// <param name="accountId">Account Id as Guid</param>
    /// <param name="fromAtUtc">Filtering starts form this DateTime(UTC)</param>
    /// <returns> List of transaction </returns>
    /// <response code="200">Returns the found transactions</response>
    /// <response code="404">If the account wasn't found</response>
    /// <response code="401">If the user haven't logined in the system</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("all/{accountId:guid}/{fromAtUtc:datetime}"), Authorize]
    public async Task<IActionResult> GetAnExtract([Required] Guid accountId, [Required] DateTime fromAtUtc)
    {
        if (claimsService.TryGetUserId(User, out var userId) == false)
            return Unauthorized(MbResult<List<TransactionDto>>.Fail("You haven't entered in the system"));

        try
        {
            List<TransactionDto> transactions =
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
    /// <response code="401">If the user haven't logined in the system</response>
    /// <response code="402">Balance of Account or Counterparty Account is less than the sum of transaction</response>
    /// <response code="403">Whether a User try to take someone else's money OR a User isn't the owner of it</response>
    /// <response code="404">If the account or the counterparty account wasn't found</response> 
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
    [HttpPost("create"), ValidationFilter, Authorize]
    public async Task<IActionResult> CreateTransaction([Required, FromForm] TransactionCreateRequest request)
    {
        if (request.AccountId == request.CounterpartyAccountId)
            return BadRequest(MbResult<Guid>.Fail("AccountId and CounterpartyAccountId are same"));

        if (claimsService.TryGetUserId(User, out var userId) == false)
            return Unauthorized(MbResult<List<TransactionDto>>.Fail("You haven't entered in the system"));

        try
        {
            var description = DescriptionValueObject.Create(request.Description);

            if (description == null)
                return BadRequest(MbResult<Guid>.Fail("Description is empty or more than 5.000 characters"));

            var currency = CurrencyValueObject.Create(request.IsoCurrencyCode);

            if (currency == null)
                return BadRequest(MbResult<Guid>.Fail($"The Currency code({request.IsoCurrencyCode}) isn't allowed"));

            var result = TransactionCreateCommand.Create(userId, request.AccountId,
                request.Sum, request.TransactionType,
                currency, description, request.CounterpartyAccountId);

            if (result.IsSuccess == false)
                return BadRequest(MbResult<Guid>.Fail(result.ErrorMessage));

            Guid id = await mediator.Send(result.Result);
            return CreatedAtAction(nameof(GetTransaction), new { id = id }, id);
        }
        catch (BadRequestExсeption exception)
        {
            return BadRequest(exception.ToMbResult<Guid>());
        }
        catch (NotFoundException exception)
        {
            return NotFound(exception.ToMbResult<Guid>());
        }
        catch (ForbiddenException exception)
        {
            logger.LogTrace($"Forbidden: {exception.Message}");
            return StatusCode(StatusCodes.Status403Forbidden, exception.ToMbResult<Guid>());
        }
        catch (PaymentRequiredException exception)
        {
            return StatusCode((int)HttpStatusCode.PaymentRequired,
                new { message = exception.ToMbResult<Guid>() });
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
    /// <response code="401">If the user haven't logined in the system</response>
    /// <response code="402">Balance of Account or Counterparty Account is less than the sum of transaction</response>
    /// <response code="403">Whether a User try to take someone else's money OR a User isn't the owner of it</response>
    /// <response code="404">If the account or the counterparty account wasn't found</response> 
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
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
            Description = request.Description,
        };

        return await CreateTransaction(transactionCreateRequest);
    }
}