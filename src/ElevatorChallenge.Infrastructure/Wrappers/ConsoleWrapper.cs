namespace ElevatorChallenge.Infrastructure.Wrappers;

public class ConsoleWrapper : IConsoleWrapper
{
    public void Clear() => Console.Clear();
    public void Write(string value) => Console.Write(value);
    public string ReadLine() => Console.ReadLine();
}