namespace ElevatorChallenge.Infrastructure.UserInterface;

public class ConsoleUserInterface : IConsoleUserInterface
{
    public void DisplayElevatorStatus(Building building)
    {
        Console.Clear();
        foreach (var elevator in building.Elevators)
        {
            Console.WriteLine($"Elevator {elevator.Id}: Floor {elevator.CurrentFloor}, Status: {elevator.Status}, Direction {elevator.Direction}");
        }
    }

    public async Task<string?> GetUserInputAsync()
    {
        Console.Write("Enter command (e.g. 'call 5 up' or 'quit'): ");
        return await Task.Run(() => Console.ReadLine());
    }

    public void DisplayMessage(string message)
    {
        Console.WriteLine(message);
    }
}