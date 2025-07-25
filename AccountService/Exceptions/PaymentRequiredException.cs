namespace AccountService.Exceptions;

public class PaymentRequiredException : Exception
{
    public PaymentRequiredException(string message) : base(message)
    {
        
    }
}