namespace OnlineBookstore.Application.Exceptions;

public class OrderClosedException : Exception
{
    public OrderClosedException(string msg)
    : base(msg)
    {
    }
}