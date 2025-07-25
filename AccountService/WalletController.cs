using System.ComponentModel.DataAnnotations;
using AccountService.Commands.CreateWallet;
using AccountService.Commands.DeleteWallet;
using AccountService.Commands.PartiallyUpdateWallet;
using AccountService.Commands.UpdateWallet;
using AccountService.Data;
using AccountService.Domain.Entities;
using AccountService.Domain.ValueObjects;
using AccountService.DTOs;
using AccountService.Exceptions;
using AccountService.Filters;
using AccountService.Queries.GetAllWallets;
using AccountService.Queries.GetWallet;
using AccountService.Queries.HasWalletBeenUsed;
using AccountService.Requests;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService;

[ApiController]
[Route("api/wallets")]
public class WalletController(
    IMediator mediator,
    IMapper mapper,
    ILogger<WalletController> logger) : ControllerBase
{
    // TODO
    // Позже когда Id клиента будет из токена браться нужно переделать GetAll() и Get(), им в таком случаи не будет смысла брать из query id-шник

    /// <summary>
    /// Get an Account By ID
    /// </summary>
    /// <returns> The List of transaction </returns>
    /// <response code="200">Returns the found account</response>
    /// <response code="404">If the account wasn't found</response>
    /// <response code="403">If you aren't the owner</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpGet("{id}/{ownerId}", Name = nameof(Get)), ValidationFilter]
    public async Task<IActionResult> Get([Required] Guid id, [Required] Guid ownerId)
    {
        WalletEntity? foundWallet;
        try
        {
            foundWallet = await mediator.Send(new GetWalletQuery(id: id, ownerId: ownerId));
        }
        catch (ForbiddenException exception)
        {
            return BadRequest($"Forbidden: {exception.Message}");
        }
        catch (NotFoundException exception)
        {
            return NotFound(exception.Message);
        }

        var dto = mapper.Map<WalletDto>(foundWallet);
        return Ok(dto);
    }

    /// <summary>
    /// Get an accounts of the owner
    /// </summary>
    /// <returns>Returns the accounts of the owner</returns>
    /// <response code="200">Returns the accounts of the owner</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("all/{ownerId}"), ValidationFilter]
    public async Task<IActionResult> GetAll([Required] Guid ownerId)
    {
        var wallets = await mediator.Send(new GetAllWallestQuery(ownerId));
        return Ok(wallets);
    }

    /// <summary>
    /// Creates a wallet
    /// </summary>
    /// <returns> Returns an ID of the new wallet </returns>
    /// <response code="201">Returns an ID of the new wallet</response>
    /// <response code="400">Whether the Currency Code doesn't comply with ISO 4217 format OR a User take invalid data</response>
    /// <response code="404">If the Owner hasn't been found</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost("create"), ValidationFilter]
    public async Task<IActionResult> Create([Required, FromForm] WalletCreateRequest request)
    {
        var currencyValueObject = CurrencyValueObject.Create(request.IsoCurrency);

        if (currencyValueObject == null)
            return BadRequest($"The {request.IsoCurrency} was invalid");

        var command = CreateWalletCommand.Create(request.OwnerId, request.Type, currencyValueObject, request.Balance,
            interestRate: request.InterestRate, request.ClosedAtUtc);

        if (command == null)
            return BadRequest("Invalid Data");

        Guid id;
        try
        {
            id = await mediator.Send(command);
        }
        catch (NotFoundException exception)
        {
            return NotFound(exception.Message);
        }

        return CreatedAtAction(nameof(Get), new { id = id, ownerId = request.OwnerId }, id);
    }


    /// <summary>
    /// Updates a wallet
    /// </summary>
    /// <response code="200">Updates the wallet</response>
    /// <response code="404">If the wallet wasn't found</response>
    /// <response code="403">If a User isn't the owner of it</response>
    /// <response code="400">Whether a User try update the wallet which has already been used
    /// OR the New Currency Code doesn't comply with ISO 4217 format
    /// OR a User take invalid data</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("update"), ValidationFilter]
    public async Task<IActionResult> Update([Required, FromForm] WalletUpdateRequest request)
    {
        // TODO
        // сделать проверку в handler-е на то являются ли юзер owner-ом
        // айди брать буду далее из JWT

        try
        {
            var hasBeenUsed = await mediator.Send(new HasWalletBeenUsedQuery(request.Id));

            if (hasBeenUsed)
                return BadRequest("You can't update the wallet because it has already been used");

            var newCurrency = CurrencyValueObject.Create(request.NewIsoCurrencyCode);

            if (newCurrency == null)
                return BadRequest($"The New {nameof(request.NewIsoCurrencyCode)} was invalid");

            var command = UpdateWalletCommand.Create(request.Id, request.NewType, newCurrency, request.NewBalance,
                newInterestRate: request.NewInterestRate, request.ClosedAtUtc);

            if (command == null)
                return BadRequest("Invalid Data");

            await mediator.Send(command);
        }
        catch (NotFoundException exception)
        {
            logger.LogTrace(exception.Message);
            return NotFound("The Wallet wasn't found");
        }
        catch (ForbiddenException exception)
        {
            logger.LogTrace(exception.Message);
            return BadRequest($"Forbidden: {exception.Message}");
        }

        return Ok();
    }

    /// <summary>
    /// Partially Updates an Account
    /// </summary>
    /// <returns> The List of transaction </returns>
    /// <response code="200">Updates the wallet partially</response>
    /// <response code="404">If the wallet wasn't found</response>
    /// <response code="400">Whether the wallet's already been deleted
    /// OR the wallet have a WalletType.Checking type and the NewInterestRate's been not null
    /// OR the wallet have other type and the NewInterestRate's been null</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPatch("update-interest-rate"), ValidationFilter]
    public async Task<IActionResult> PartiallyUpdate([Required, FromForm] WalletPartiallyUpdateRequest request)
    {
        // TODO
        // сделать проверку в handler-е на то являются ли юзер owner-ом
        // айди брать буду далее из JWT

        if (request.ClosedAtUtc < DateTime.UtcNow)
            return BadRequest("The closed time should be in the future");

        try
        {
            await mediator.Send(
                new PartiallyUpdateWalletCommand(request.Id, request.NewInterestRate, request.ClosedAtUtc));
        }
        catch (NotFoundException exception)
        {
            logger.LogTrace(exception.Message);
            return NotFound(exception.Message);
        }
        catch (BadRequestExсeption exception)
        {
            logger.LogTrace(exception.Message);
            return BadRequest(exception.Message);
        }
        catch (ForbiddenException exception)
        {
            logger.LogTrace(exception.Message);
            return BadRequest($"Forbidden: {exception.Message}");
        }

        return Ok();
    }

    /// <summary>
    /// Deletes Account
    /// </summary>
    /// <response code="200">The Account's been deleted</response>
    /// <response code="404">If the account wasn't found</response>
    /// <response code="403">If you aren't the owner</response>
    /// <response code="400">If the account's already been deleted</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpDelete("delete/{id}"), ValidationFilter]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await mediator.Send(new DeleteWalletCommand(id));
        }
        catch (NotFoundException exception)
        {
            logger.LogTrace(exception.Message);
            return NotFound("The Wallet wasn't found");
        }
        catch (BadRequestExсeption exception)
        {
            return BadRequest(exception.Message);
        }
        catch (ForbiddenException exception)
        {
            logger.LogTrace(exception.Message);
            return BadRequest($"Forbidden: {exception.Message}");
        }

        return Ok();
    }
}