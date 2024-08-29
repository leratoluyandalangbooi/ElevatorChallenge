namespace ElevatorChallenge.Application.Services;

public class SimulationEngine
{
    private readonly Building _building;
    private readonly IElevatorService _elevatorService;
    private readonly IConsoleUserInterface _consoleUserInterface;

    public SimulationEngine(Building building, IElevatorService elevatorService, IConsoleUserInterface consoleUserInterface)
    {
        _building = building;
        _elevatorService = elevatorService;
        _consoleUserInterface = consoleUserInterface;
    }

    public async Task RunSimulation()
    {
        while (true)
        {
            UpdateElevatorStatus();
            _consoleUserInterface.DisplayElevatorStatus(_building);
            await HandleUserInputAsync();
            await Task.Delay(1000);
        }
    }

    private void UpdateElevatorStatus()
    {
        foreach (var elevator in _building.Elevators)
        {
            if (elevator.IsMoving)
            {
                elevator.UpdateElevatorPosition();
            }
        }
    }
    
    private async Task HandleUserInputAsync()
    {
        var input = await _consoleUserInterface.GetUserInputAsync();

        if (input == null)
            throw new InputValidationException("Unknown or incorrect user input");

        var inputParts = input.Split(' ');

        switch (inputParts[0].ToLower())
        {
            case "call":
                await CallElevatorAsync(int.Parse(inputParts[1]), Enum.Parse<Direction>(inputParts[2], true));
                break;

            case "move":
                await MoveElevatorAsync(int.Parse(inputParts[1]), int.Parse(inputParts[2]));
                break;

            case "add":
                AddPassenger(int.Parse(inputParts[1]), int.Parse(inputParts[2]));
                break;

            case "quit":
                Environment.Exit(0);
                break;
            default:
                throw new InputValidationException("Unknown command");
        }
    }

    private async Task CallElevatorAsync(int floor, Direction direction)
    {
        await _elevatorService.DispatchElevatorAsync(floor, direction); 
    }

    private async Task MoveElevatorAsync(int elevatorId, int destinationFloor)
    {
        var elevator = _building.GetElevator(elevatorId);
        await _elevatorService.MoveElevatorAsync(elevator, destinationFloor);
    }

    private void AddPassenger(int currentFloor, int destinationFloor)
    {
        var floor = _building.GetFloor(currentFloor);
        var generatePassengerId = Guid.NewGuid().GetHashCode();
        var random = new Random();
        var generatePassengerWeight = random.Next(20, 200); //randomise weight between 20 and 200kg
        var passenger = new Passenger(generatePassengerId, destinationFloor, generatePassengerWeight);
        floor.AddWaitingPassenger(passenger);
    }
}