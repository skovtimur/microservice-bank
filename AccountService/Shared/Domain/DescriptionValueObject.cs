namespace AccountService.Shared.Domain;

public class DescriptionValueObject
{
    public required string Description { get; init; }

    public static MbResult<DescriptionValueObject> Create(string description)
    {
        var valueObject = new DescriptionValueObject { Description = description };
        var result = DescriptionValidator.IsValid(valueObject);
        
        // ReSharper disable once NullableWarningSuppressionIsUsed
        // раз не валидно значит есть причина почему

        return result.IsSuccess
            ? MbResult<DescriptionValueObject>.Ok(valueObject)
            : MbResult<DescriptionValueObject>.Fail(result.Error!);
    }

    public override string ToString()
    {
        return Description;
    }
}