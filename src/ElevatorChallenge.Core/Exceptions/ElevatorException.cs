namespace ElevatorChallenge.Core.Exceptions;

public class ElevatorException : Exception
{
    public ElevatorException() { }

    public ElevatorException(string message) : base(message) { }

    public ElevatorException(string? message, Exception? innerException) : base(message, innerException) { }
}
