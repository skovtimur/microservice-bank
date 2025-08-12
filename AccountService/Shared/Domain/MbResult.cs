namespace AccountService.Shared.Domain;

public readonly struct MbResult<TSuccess>
{
    public bool IsSuccess { get; private init; }
    public TSuccess? Result { get; private init; }
    public MbError? Error { get; private init; }
    

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    // Getter может пригодится
    public DateTime CreatedAtUtc { get; private init; }


    public static MbResult<TSuccess> Ok(TSuccess result)
    {
        if (result is null)
            throw new ArgumentNullException(nameof(result));

        return new MbResult<TSuccess>
        {
            IsSuccess = true,
            Result = result,
            Error = null,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public static MbResult<TSuccess> Fail(MbError error)
    {
        return new MbResult<TSuccess>
        {
            IsSuccess = false,
            Result = default,
            Error = error,
            CreatedAtUtc = DateTime.UtcNow
        };
    }
    
    public static MbResult<TSuccess> Fail(string errorMessage)
    {
        return new MbResult<TSuccess>
        {
            IsSuccess = false,
            Result = default,
            Error = new MbError(errorMessage),
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}