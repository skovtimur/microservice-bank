namespace AccountService.Shared.Domain;

// ReSharper disable once ClassNeverInstantiated.Global
// не вижу смысла делать его абстрактным как советует resharper
public class MbError
{
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    // getter-ы возможно будут нужны
    public string ErrorMessage { get; private init; }
    public string? Source { get; private init; }
    public string? HelpLink { get; private init; }
    public string? StackTrace { get; private init; }
    public MbError? InnerException { get; private init; }


    // ReSharper disable UnusedMember.Global
    // ReSharper disable MemberCanBePrivate.Global
    // Удалять конструкторы или делать их private глупо тк может пригодятся именно такое расширение конструктора
    public MbError(string exceptionMessage, Exception exception)
    {
        ErrorMessage = exception.Message;
        StackTrace = exception.StackTrace;
        Source = exception.Source;
        HelpLink = exception.HelpLink;
        ErrorMessage = exceptionMessage;
        InnerException = GetInnerError(exception);
    }

    // может пригодится тоже
    public MbError(Exception exception)
    {
        ErrorMessage = exception.Message;
        StackTrace = exception.StackTrace;
        Source = exception.Source;
        HelpLink = exception.HelpLink;
        InnerException = GetInnerError(exception);
    }

    public MbError(string exceptionMessage)
    {
        ErrorMessage = exceptionMessage;
    }

    private static MbError? GetInnerError(Exception? exception)
    {
        var innerException = exception?.InnerException;

        return innerException == null
            ? null
            : new MbError(innerException);
    }
}