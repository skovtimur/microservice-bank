using FluentValidation;

namespace AccountService.Shared.Domain;

public class DescriptionValidator : AbstractValidator<DescriptionValueObject>
{
    public const int MaxDescriptionLength = 5000;
    public const int MinDescriptionLength = 3;

    public DescriptionValidator()
    {
        RuleFor(x => x.Description).NotEmpty()
            .NotNull().WithMessage("Description is empty")
            .MinimumLength(MinDescriptionLength)
            .MaximumLength(MaxDescriptionLength)
            .WithMessage($"Description length must be between {MinDescriptionLength} and {MaxDescriptionLength}");
    }

    private static readonly DescriptionValidator Validator = new()
    {
        ClassLevelCascadeMode = CascadeMode.Continue,
        RuleLevelCascadeMode = CascadeMode.Stop
    };

    public static MbResult<bool> IsValid(DescriptionValueObject description)
    {
        var result = Validator.Validate(description);
        var firstError = result.Errors.FirstOrDefault();

        // ReSharper disable once NullableWarningSuppressionIsUsed
        // раз не валидно значит есть причина почему
        
        return result.IsValid
            ? MbResult<bool>.Ok(true)
            : MbResult<bool>.Fail(new MbError(firstError!.ErrorMessage));
    }
}