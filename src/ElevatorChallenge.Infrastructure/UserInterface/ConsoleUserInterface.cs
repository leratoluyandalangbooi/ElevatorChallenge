using ElevatorChallenge.Core.Enums;
using ElevatorChallenge.Infrastructure.Wrappers;
using System.Collections.Concurrent;
using System.Text;

namespace ElevatorChallenge.Infrastructure.UserInterface;

public class ConsoleUserInterface : IConsoleUserInterface
{
    private readonly ConcurrentQueue<string> _messageQueue = new ConcurrentQueue<string>();
    private string _currentDisplay = "";
    private readonly IConsoleWrapper _console;

    public ConsoleUserInterface(IConsoleWrapper console = null)
    {
        _console = console ?? new ConsoleWrapper();
    }

    public void DisplayElevatorStatus(Building building)
    {
        StringBuilder sb = new StringBuilder();
        DisplayElevatorDetails(building, sb);
        DisplayInstructions(sb);
        string newDisplay = sb.ToString();
        if (newDisplay != _currentDisplay)
        {
            _console.Clear();
            _console.Write(newDisplay);
            _currentDisplay = newDisplay;
        }
    }

    private void DisplayInstructions(StringBuilder sb)
    {
        sb.AppendLine("\nRecent Messages:");
        sb.AppendLine("──────────────────");
        foreach (var message in _messageQueue.TakeLast(3))
        {
            sb.AppendLine(message);
        }

        sb.AppendLine("\nAvailable Commands:");
        sb.AppendLine("────────────────────");
        sb.AppendLine("call <floor> <up/down>    : Call elevator (e.g., 'call 5 up')");
        sb.AppendLine("move <elevatorId> <floor> : Move elevator (e.g., 'move 1 3')");
        sb.AppendLine("add <floor> <destFloor>   : Add passenger (e.g., 'add 1 5')");
        sb.AppendLine("quit                      : Exit simulation");

        sb.Append("\nEnter command:");
    }

    private static void DisplayElevatorDetails(Building building, StringBuilder sb)
    {
        sb.AppendLine("Elevator Simulation Engine".ToUpper());
        sb.AppendLine("────────────────────────────");
        sb.AppendLine("\nElevator Status:");
        sb.AppendLine("────────────────");
        foreach (var elevator in building.Elevators)
        {
            sb.AppendLine($"Elevator {elevator.Id}/{building.Elevators.Count}: Floor {elevator.CurrentFloor}/{building.Floors.Count} | {elevator.Status} | {elevator.Direction} | Passenger:{elevator.Passengers.Count}/{elevator.Capacity}");
        }

        sb.AppendLine("\nFloor Requests:");
        sb.AppendLine("─────────────────");
        for (int i = building.Floors.Count; i > 0; i--)
        {
            var floor = building.GetFloor(i);
            if (floor.ElevatorCallButtons[Direction.Up] == true || floor.ElevatorCallButtons[Direction.Down] == true || floor.WaitingPassengersCount > 0)
            {
                sb.Append($"Floor {i}: ");
                sb.Append(floor.ElevatorCallButtons[Direction.Up] == true ? "▲ " : "  ");
                sb.Append(floor.ElevatorCallButtons[Direction.Down] == true ? "▼ " : "  ");
                sb.AppendLine($"Waiting: {floor.WaitingPassengersCount}");
            }
        }
    }
    public async Task<string?> GetUserInputAsync()
    {
        _console.Write("Enter command: ");
        return await Task.Run(() => _console.ReadLine());
    }

    public void DisplayMessage(string message)
    {
        _messageQueue.Enqueue(message);
        if (_messageQueue.Count > 10) // Keep only last 10 messages
        {
            _messageQueue.TryDequeue(out _);
        }
    }

    public void Initialize()
    {
        _console.Clear();
    }
}