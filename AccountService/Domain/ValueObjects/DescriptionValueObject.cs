using AccountService.Validators;

namespace AccountService.Domain.ValueObjects;

public class DescriptionValueObject
{
    public required string Description { get; init; }

    public static DescriptionValueObject? Create(string description)
    {
        return string.IsNullOrEmpty(description) ||
               description.Length > TransactionCreateCommandValidator.MaxDescriptionLength
            ? null
            : new DescriptionValueObject { Description = description };
    }
}