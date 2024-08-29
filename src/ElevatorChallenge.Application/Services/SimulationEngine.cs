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
            switch (elevator.Status)
            {
                case ElevatorStatus.Idle:
                    HandleIdleElevator(elevator);
                    break;
                case ElevatorStatus.Moving:
                    HandleMovingElevator(elevator);
                    break;
                case ElevatorStatus.Loading:
                    HandleLoadingElevator(elevator);
                    break;
                case ElevatorStatus.Unloading:
                    HandleUnloadingElevator(elevator);
                    break;
                default:
                    throw new ElevatorException("Elevator out of service");
            }
        }
    }

    private void HandleIdleElevator(Elevator elevator)
    {
        var pendingElevatorRequest = _building.Floors
                                              .FirstOrDefault(f => f.ElevatorCallButtons[Direction.Up] || f.ElevatorCallButtons[Direction.Down]);
        if (pendingElevatorRequest != null)
        {
            elevator.SetDestination(pendingElevatorRequest.FloorNumber);
            elevator.SetElevatorStatus(ElevatorStatus.Moving);
        }
    }

    private void HandleMovingElevator(Elevator elevator)
    {
        elevator.MoveElevator(elevator.DestinationFloor);
        if (elevator.CurrentFloor == elevator.DestinationFloor)
        {
            elevator.SetElevatorStatus(ElevatorStatus.Unloading);
        }
    }

    private void HandleLoadingElevator(Elevator elevator)
    {
        var currentFloor = _building.GetFloor(elevator.CurrentFloor);

        while (currentFloor.WaitingPassengersCount > 0 && elevator.CanAddPassenger(currentFloor.WaitingPassengers.Peek()))
        {
            var passenger = currentFloor.RemoveWaitingPassenger();
            elevator.AddPassenger(passenger!);
        }

        if (elevator.Passengers.Any())
        {
            elevator.SetElevatorStatus(ElevatorStatus.Moving);
            int nextDestination = elevator.Passengers.Min(p => p.DestinationFloor);
            elevator.SetDestination(nextDestination);
        }
        else if (!currentFloor.ElevatorCallButtons[Direction.Up] && !currentFloor.ElevatorCallButtons[Direction.Down])
        {
            elevator.SetElevatorStatus(ElevatorStatus.Idle);
        }
    }
    
    private void HandleUnloadingElevator(Elevator elevator)
    {
        var passengersToRemove = elevator.Passengers.Where(p => p.DestinationFloor == elevator.CurrentFloor).ToList();
        foreach (var passenger in passengersToRemove)
        {
            elevator.RemovePassenger(passenger);
        }

        if (elevator.Passengers.Any())
        {
            elevator.SetElevatorStatus(ElevatorStatus.Moving);
            int nextDestination = elevator.Direction == Direction.Up
                ? elevator.Passengers.Min(p => p.DestinationFloor)
                : elevator.Passengers.Max(p => p.DestinationFloor);
            elevator.SetDestination(nextDestination);
        }
        else
        {
            var currentFloor = _building.GetFloor(elevator.CurrentFloor);
            if (currentFloor.WaitingPassengersCount > 0)
            {
                elevator.SetElevatorStatus(ElevatorStatus.Loading);
            }
            else
            {
                elevator.SetElevatorStatus(ElevatorStatus.Idle);
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
        elevator.SetDestination(destinationFloor);
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