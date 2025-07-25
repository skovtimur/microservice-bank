using System.ComponentModel.DataAnnotations;
using System.Net;
using AccountService.Commands.CreateTransaction;
using AccountService.Domain;
using AccountService.Domain.Entities;
using AccountService.Domain.ValueObjects;
using AccountService.DTOs;
using AccountService.Exceptions;
using AccountService.Filters;
using AccountService.Queries.GetAllTransactions;
using AccountService.Queries.GetTransaction;
using AccountService.Requests;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService;

[ApiController]
[Route("transactions")]
public class TransactionController(
    IMapper mapper,
    IMediator mediator,
    ILogger<TransactionController> logger) : ControllerBase
{
    // TODO
    // Как будет реализованна авторизация нужно добавить проверку является ли это owner-ом

    /// <summary>
    /// Get a transaction by id
    /// </summary>
    /// <returns> Found transaction </returns>
    /// <response code="200">Returns the found transaction</response>
    /// <response code="404">If the transaction doesn't exist</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("{id:guid}", Name = nameof(GetTransaction))]
    public async Task<IActionResult> GetTransaction(Guid id)
    {
        TransactionEntity? transaction = await mediator.Send(new GetTransactionQuery(id));

        if (transaction == null)
            return NotFound("Transaction not found");

        var dto = mapper.Map<TransactionDto>(transaction);
        return Ok(dto);
    }

    /// <summary>
    /// Get a transaction by Account ID
    /// </summary>
    /// <returns> List of transaction </returns>
    /// <response code="200">Returns the found transactions</response>
    /// <response code="404">If the account wasn't found</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("all/{accountId:guid}/{fromAtUtc:datetime}")]
    public async Task<IActionResult> GetAnExtract([Required] Guid accountId, [Required] DateTime fromAtUtc)
    {
        List<TransactionDto> transactions;

        try
        {
            transactions = await mediator.Send(new GetAllTransactionsQuery(accountId, fromAtUtc));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }

        return Ok(transactions);
    }

    /// <summary>
    /// Creates a transaction
    /// </summary>
    /// <response code="200">Creates the transaction</response>
    /// <response code="400">Whether the Description is empty or more than 5.000 characters
    /// OR the Currency Code doesn't comply with ISO 4217 format
    /// OR a User take invalid data
    /// OR the account or the counterparty account has already been deleted</response>
    /// <response code="402">Balance of Account or Counterparty Account is less than the sum of transaction</response>
    /// <response code="403">Whether a User try to take someone else's money OR a User isn't the owner of it</response>
    /// <response code="404">If the account or the counterparty account wasn't found</response> 
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
    [HttpPost("create"), ValidationFilter]
    public async Task<IActionResult> CreateTransaction([Required, FromForm] TransactionCreateRequest request)
    {
        if (request.AccountId == request.CounterpartyAccountId)
            return BadRequest("AccountId and CounterpartyAccountId are same");

        Guid id;

        try
        {
            var description = DescriptionValueObject.Create(request.Description);
            if (description == null)
                return BadRequest("Description is empty or more than 5.000 characters");

            var currency = CurrencyValueObject.Create(request.IsoCurrencyCode);

            if (currency == null)
                return BadRequest($"The Currency code({request.IsoCurrencyCode}) doesn't exist");

            var command = TransactionCreateCommand.Create(request.OwnerId, request.AccountId,
                request.Sum, request.TransactionType,
                currency, description, request.CounterpartyAccountId);

            if (command == null)
                return BadRequest("The Transaction is invalid");

            id = await mediator.Send(command);
        }
        catch (BadRequestExсeption exception)
        {
            return BadRequest(exception.Message);
        }
        catch (NotFoundException exception)
        {
            return NotFound(exception.Message);
        }
        catch (ForbiddenException exception)
        {
            logger.LogTrace($"Forbidden: {exception.Message}");
            return BadRequest($"Forbidden: {exception.Message}");
        }
        catch (PaymentRequiredException exception)
        {
            return StatusCode((int)HttpStatusCode.PaymentRequired,
                new { message = exception.Message });
        }


        return CreatedAtAction(nameof(GetTransaction), new { id = id }, id);
    }

    [HttpPost("transfer-money"), ValidationFilter]
    public async Task<IActionResult> TransferMoney([Required, FromForm] TransferMoneyRequest request)
    {
        var transactionCreateRequest = new TransactionCreateRequest
        {
            OwnerId = request.OwnerId,
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