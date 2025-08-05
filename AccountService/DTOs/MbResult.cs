namespace AccountService.DTOs;

public readonly struct MbResult<T>
{
    public bool IsSuccess { get; private init; }
    public T? Result { get; private init; }
    public string? ErrorMessage { get; private init; }
    public DateTime CreatedAtUtc { get; private init; }


    public static MbResult<T> Ok(T result)
    {
        if (result is null)
            throw new ArgumentNullException(nameof(result));

        return new MbResult<T>
        {
            IsSuccess = true,
            Result = result,
            ErrorMessage = null,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public static MbResult<T> Fail(string? errorMessage)
    {
        return new MbResult<T>
        {
            IsSuccess = false,
            Result = default,
            ErrorMessage = errorMessage,
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}