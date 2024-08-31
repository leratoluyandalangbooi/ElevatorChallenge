using ElevatorChallenge.Core.Entities;
using System.IO;

namespace ElevatorChallenge.Application.Services;

public class SimulationEngine
{
    private readonly Building _building;
    private readonly IElevatorService _elevatorService;
    private readonly IConsoleUserInterface _consoleUserInterface;
    private bool _isRunning = true;
    private readonly object _lock = new object();

    public SimulationEngine(Building building, IElevatorService elevatorService, IConsoleUserInterface consoleUserInterface)
    {
        _building = building;
        _elevatorService = elevatorService;
        _consoleUserInterface = consoleUserInterface;
    }

    public async Task RunSimulation()
    {
        _consoleUserInterface.Initialize();
        var uiUpdateTask = Task.Run(UpdateUILoop);
        var simulationTask = Task.Run(SimulationLoop);
        var userInputTask = HandleUserInputLoop();

        await Task.WhenAll(uiUpdateTask, simulationTask, userInputTask);
    }

    private async Task UpdateUILoop()
    {
        while (_isRunning)
        {
            _consoleUserInterface.DisplayElevatorStatus(_building);
            await Task.Delay(100); // Update UI every 100ms
        }
    }

    private async Task SimulationLoop()
    {
        while (_isRunning)
        {
            lock (_lock)
            {
                UpdateElevatorPositions();
            }
            await Task.Delay(500); // Update simulation every 500ms
        }
    }

    private void UpdateElevatorPositions()
    {
        foreach (var elevator in _building.Elevators)
        {
            if (elevator.IsMoving)
            {
                elevator.UpdateElevatorPosition();
                if (elevator.CurrentFloor == elevator.DestinationFloor)
                {
                    _elevatorService.OnElevatorArrivedAsync(elevator);
                }
            }
        }
    }

    private async Task HandleUserInputLoop()
    {
        while (_isRunning)
        {
            var input = await _consoleUserInterface.GetUserInputAsync();
            if (input == null)
                throw new InputValidationException("Unknown or incorrect user input");

            await HandleUserInputAsync(input);
        }
    }

    private async Task HandleUserInputAsync(string input)
    {
        var inputParts = input.Split(' ');
        var num = -1;
        try
        {
            switch (inputParts[0].ToLower())
            {
                case "call":
                    if (inputParts.Length != 3 || int.TryParse(inputParts[2], out num)) throw new ArgumentException("Invalid call command. Usage: call <floor> <direction (up/down)>");
                    await CallElevatorAsync(int.Parse(inputParts[1]), Enum.Parse<Direction>(inputParts[2], true));
                    break;

                case "move":
                    if (inputParts.Length != 3) throw new ArgumentException("Invalid move command. Usage: move <elevatorId> <floor>");
                    await MoveElevatorAsync(int.Parse(inputParts[1]), int.Parse(inputParts[2]));
                    break;

                case "add":
                    if (inputParts.Length != 3) throw new ArgumentException("Invalid add command. Usage: add <floor> <destinationFloor>");
                    AddPassenger(int.Parse(inputParts[1]), int.Parse(inputParts[2]));
                    break;

                case "quit":
                    _isRunning = false;
                    break;

                default:
                    throw new InputValidationException("Unknown command");
            }
        }
        catch (Exception ex)
        {
            _consoleUserInterface.DisplayMessage($"Error: {ex.Message}");
        }
    }

    private async Task CallElevatorAsync(int floor, Direction direction)
    {
        await _elevatorService.DispatchElevatorAsync(floor, direction);
        _consoleUserInterface.DisplayMessage($"Elevator called to floor {floor}, direction: {direction}");
    }

    private async Task MoveElevatorAsync(int elevatorId, int destinationFloor)
    {
        var elevator = _building.Elevators.FirstOrDefault(e => e.Id == elevatorId);
        if (elevator == null)
        {
            _consoleUserInterface.DisplayMessage($"Elevator {elevatorId} not found");
            return;
        }
        await _elevatorService.MoveElevatorAsync(elevator, destinationFloor);
        _consoleUserInterface.DisplayMessage($"Elevator {elevatorId} moving to floor {destinationFloor}");
    }

    private void AddPassenger(int currentFloor, int destinationFloor)
    {
        var floor = _building.GetFloor(currentFloor);
        if (floor == null)
        {
            _consoleUserInterface.DisplayMessage($"Floor {currentFloor} not found");
            return;
        }

        var generatePassengerId = Guid.NewGuid().GetHashCode();
        var random = new Random();
        var generatePassengerWeight = random.Next(20, 200); //randomise weight between 20 and 200kg
        var passenger = new Passenger(generatePassengerId, destinationFloor, generatePassengerWeight);
        floor.AddWaitingPassenger(passenger);
        _consoleUserInterface.DisplayMessage($"Passenger added to floor {currentFloor}, destination: {destinationFloor}");
    }
}