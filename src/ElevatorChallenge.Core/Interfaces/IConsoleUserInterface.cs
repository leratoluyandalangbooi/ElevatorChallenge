namespace ElevatorChallenge.Core.Interfaces;

public interface IConsoleUserInterface
{
    void DisplayElevatorStatus(Building building);
    Task<string?> GetUserInputAsync();
    void DisplayMessage(string message);
}
