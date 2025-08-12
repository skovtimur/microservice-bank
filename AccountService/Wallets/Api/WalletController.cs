using System.ComponentModel.DataAnnotations;
using AccountService.Shared.Abstractions.ServiceInterfaces;
using AccountService.Shared.Api.Filters;
using AccountService.Shared.Domain;
using AccountService.Shared.Exceptions;
using AccountService.Wallets.Api.Requests;
using AccountService.Wallets.CreateWallet;
using AccountService.Wallets.DeleteWallet;
using AccountService.Wallets.Domain;
using AccountService.Wallets.GetAllWallets;
using AccountService.Wallets.GetWallet;
using AccountService.Wallets.PartiallyUpdateWallet;
using AccountService.Wallets.UpdateWallet;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Wallets.Api;

[ApiController]
[Route("/api/wallets")]
public class WalletController(
    IMediator mediator,
    IClaimsService claimsService) : ControllerBase
{
    // ReSharper disable NullableWarningSuppressionIsUsed
    //Чтобы не блокировали Оператор ! (null-forgiving operator), а точнее я использую MbResult где Result может быть null ТОЛЬКО ПРИ isSuccess = false,
    //я же делаю проверку поэтому данный warning излишен

    /// <summary>
    /// Get an Account By ID
    /// </summary>
    /// <param name="id">Account ID as Guid</param>
    /// <returns> The List of transaction </returns>
    /// <response code="200">Returns the found account</response>
    /// <response code="401">If the user haven't logged in the system</response>
    /// <response code="404">If the account wasn't found</response>
    /// <response code="403">If you aren't the owner</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpGet("{id:guid}", Name = nameof(Get)), ValidationFilter, Authorize]
    public async Task<IActionResult> Get([Required] Guid id)
    {
        if (claimsService.TryGetUserId(User, out var userId) == false)
            return Unauthorized(MbResult<WalletDto>.Fail("You haven't entered in the system"));

        try
        {
            var foundWallet = await mediator.Send(new GetWalletQuery(id: id, ownerId: userId));
            var result = MbResult<WalletDto>.Ok(foundWallet);

            return Ok(result);
        }
        catch (ForbiddenException exception)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                exception.ToMbResult<WalletDto>());
        }
        catch (NotFoundException exception)
        {
            return NotFound(exception.ToMbResult<WalletDto>());
        }
    }

    /// <summary>
    /// Get an accounts of the owner
    /// </summary>
    /// <returns>Returns the accounts of the owner</returns>
    /// <response code="200">Returns the accounts of the owner</response>
    /// <response code="401">If the user haven't logined in the system</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("all"), ValidationFilter, Authorize]
    public async Task<IActionResult> GetAll()
    {
        if (claimsService.TryGetUserId(User, out var userId) == false)
            return Unauthorized(MbResult<List<WalletDto>>.Fail("You haven't entered in the system"));

        var wallets = await mediator.Send(new GetAllWalletsQuery(userId));
        var result = MbResult<List<WalletDto>>.Ok(wallets);

        return Ok(result);
    }

    /// <summary>
    /// Creates a wallet
    /// </summary>
    /// <returns> The new wallet ID </returns>
    /// <response code="201">Returns an ID of the new wallet</response>
    /// <response code="400">Whether the Currency Code doesn't comply with ISO 4217 format OR a User take invalid data</response>
    /// <response code="401">If the user haven't logined in the system</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost("create"), ValidationFilter, Authorize]
    public async Task<IActionResult> Create([Required, FromForm] WalletCreateRequest request)
    {
        if (claimsService.TryGetUserId(User, out var userId) == false)
            return Unauthorized(MbResult<Guid>.Fail("You haven't entered in the system"));

        var currencyValueObject = CurrencyValueObject.Create(request.IsoCurrency);

        if (currencyValueObject == null)
            return BadRequest(MbResult<Guid>.Fail($"The {request.IsoCurrency} isn't allowed"));

        var result = CreateWalletCommand.Create(userId, request.Type, currencyValueObject, request.Balance,
            interestRate: request.InterestRate, request.ClosedAtUtc);

        if (result.IsSuccess == false)
            return BadRequest(MbResult<Guid>.Fail(result.Error!));

        var walletId = await mediator.Send(result.Result!);
        return CreatedAtAction(nameof(Get), new { id = walletId }, walletId);
    }


    /// <summary>
    /// Updates a wallet
    /// </summary>
    /// <returns> The updated account ID</returns>
    /// <response code="200">Updates the wallet</response>
    /// <response code="401">If the user haven't logined in the system</response>
    /// <response code="400">Whether a User try update the wallet which has already been used
    /// OR the New Currency Code doesn't comply with ISO 4217 format
    /// OR a User take invalid data</response>
    /// <response code="403">If a User isn't the owner of it</response>
    /// <response code="404">If the wallet wasn't found</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("update"), ValidationFilter, Authorize]
    public async Task<IActionResult> Update([Required, FromForm] WalletUpdateRequest request)
    {
        if (claimsService.TryGetUserId(User, out var userId) == false)
            return Unauthorized(MbResult<Guid>.Fail("You haven't entered in the system"));

        try
        {
            var newCurrency = CurrencyValueObject.Create(request.NewIsoCurrencyCode);

            if (newCurrency == null)
                return BadRequest(MbResult<Guid>.Fail($"The New {nameof(request.NewIsoCurrencyCode)} isn't allowed"));

            var validationResult = UpdateWalletCommand.Create(request.Id, userId, request.NewType, newCurrency,
                request.NewBalance,
                newInterestRate: request.NewInterestRate, request.ClosedAtUtc);

            if (validationResult.IsSuccess == false)
                return BadRequest(MbResult<Guid>.Fail(validationResult.Error!));

            await mediator.Send(validationResult.Result!);
        }
        catch (NotFoundException exception)
        {
            return NotFound(exception.ToMbResult<Guid>());
        }
        catch (ForbiddenException exception)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                exception.ToMbResult<Guid>());
        }
        catch (BadRequestException exception)
        {
            return BadRequest(exception.ToMbResult<Guid>());
        }

        var result = MbResult<Guid>.Ok(request.Id);
        return Ok(result);
    }

    /// <summary>
    /// Partially Updates an Account
    /// </summary>
    /// <returns> The partially updated account ID</returns>
    /// <response code="200">Updates the wallet partially</response>
    /// <response code="400">Whether the wallet's already been deleted
    /// OR the wallet have a WalletType.Checking type and the NewInterestRate's been not null
    /// OR the wallet have other type and the NewInterestRate's been null</response>
    /// <response code="401">If the user haven't logined in the system</response>
    /// <response code="403">If you aren't the owner</response>
    /// <response code="404">If the wallet wasn't found</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPatch("update-interest-rate"), ValidationFilter, Authorize]
    public async Task<IActionResult> PartiallyUpdate([Required, FromForm] WalletPartiallyUpdateRequest request)
    {
        if (claimsService.TryGetUserId(User, out var userId) == false)
            return Unauthorized(MbResult<Guid>.Fail("You haven't entered in the system"));

        if (request.ClosedAtUtc < DateTime.UtcNow)
            return BadRequest(MbResult<Guid>.Fail("The closed time should be in the future"));

        try
        {
            await mediator.Send(
                new PartiallyUpdateWalletCommand(request.Id, userId, request.NewInterestRate, request.ClosedAtUtc));
        }
        catch (NotFoundException exception)
        {
            return NotFound(exception.ToMbResult<Guid>());
        }
        catch (BadRequestException exception)
        {
            return BadRequest(exception.ToMbResult<Guid>());
        }
        catch (ForbiddenException exception)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                exception.ToMbResult<Guid>());
        }

        var result = MbResult<Guid>.Ok(request.Id);
        return Ok(result);
    }

    /// <summary>
    /// Deletes Account
    /// </summary>
    /// <param name="id">Account ID as Guid</param>
    /// <returns>The deleted account ID</returns>
    /// <response code="200">The Account's been deleted</response>
    /// <response code="400">If the account's already been deleted</response>
    /// <response code="401">If the user haven't logined in the system</response>
    /// <response code="403">If you aren't the owner</response>
    /// <response code="404">If the account wasn't found</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpDelete("delete/{id:guid}"), ValidationFilter, Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (claimsService.TryGetUserId(User, out var userId) == false)
            return Unauthorized(MbResult<Guid>.Fail("You haven't entered in the system"));

        try
        {
            await mediator.Send(new DeleteWalletCommand(id, userId));
            return Ok(MbResult<Guid>.Ok(id));
        }
        catch (NotFoundException exception)
        {
            return NotFound(exception.ToMbResult<Guid>());
        }
        catch (BadRequestException exception)
        {
            return BadRequest(exception.ToMbResult<Guid>());
        }
        catch (ForbiddenException exception)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                exception.ToMbResult<Guid>());
        }
    }
}