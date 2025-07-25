namespace AccountService.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string str) : base(str)
    {
    }
    
    public NotFoundException(Type objectType, object key) : base($"Object:{objectType} with KEY={key} was not found")
    {
    }
    
    public NotFoundException(Type objectType, Guid id) : base($"Object:{objectType} with ID={id} was not found")
    {
    }
}