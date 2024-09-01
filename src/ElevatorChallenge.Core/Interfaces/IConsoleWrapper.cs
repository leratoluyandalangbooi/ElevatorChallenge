namespace ElevatorChallenge.Core.Interfaces;

public interface IConsoleWrapper
{
    void Clear();
    void Write(string value);
    string ReadLine();
}
