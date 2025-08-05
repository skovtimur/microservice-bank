using AccountService.Validators;

namespace AccountService.Domain.ValueObjects;

public class DescriptionValueObject
{
    public string Description { get; set; }

    public static DescriptionValueObject? Create(string description)
    {
        return string.IsNullOrEmpty(description) ||
               description.Length > TransactionCreateCommandValidator.MaxDescriptionLength
            ? null
            : new DescriptionValueObject { Description = description };
    }
}