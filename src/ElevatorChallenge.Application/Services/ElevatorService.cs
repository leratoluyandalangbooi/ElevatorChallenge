using ElevatorChallenge.Core.Shared;

namespace ElevatorChallenge.Application.Services;

public class ElevatorService : IElevatorService
{
    private readonly Building _building;
    private readonly INearestAvailableElevatorStrategy _elevatorDispatchStrategy;
    private readonly IElevatorLogger _logger;

    public ElevatorService(Building building, INearestAvailableElevatorStrategy elevatorDispatchStrategy, IElevatorLogger logger)
    {
        _building = building;
        _elevatorDispatchStrategy = elevatorDispatchStrategy;
        _logger = logger;
    }

    public async Task DispatchElevatorAsync(int requestedFloor, Direction direction)
    {
        InputValidator.ValidateFloorNumber(requestedFloor, _building.Floors.Count);
        InputValidator.ValidateElevatorId(requestedFloor, _building.Elevators.Count);
        InputValidator.ValidateDirection(direction);

        try
        {
            var elevator = _elevatorDispatchStrategy.SelectElevator(_building.Elevators, requestedFloor, direction);
            await MoveElevatorAsync(elevator, requestedFloor);
            await LoadPassengersAsync(elevator, requestedFloor);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error dispatching elevator", ex);
            throw;
        }
    }

    private async Task LoadPassengersAsync(Elevator elevator, int requestedFloor)
    {
        var floor = _building.GetFloor(requestedFloor);
        var passengerQueued = floor.WaitingPassengersCount > 0 && elevator.CanAddPassenger(floor.WaitingPassengers.Peek());
        while (passengerQueued)
        {
            var passenger = floor.RemoveWaitingPassenger();
            elevator.AddPassenger(passenger!);
            _logger.LogPassengerBoarding(elevator, passenger);
        }
        await Task.Delay(1000); //Loading time
    }

    public async Task MoveElevatorAsync(Elevator elevator, int destinationFloor)
    {
        var oldStatus = elevator.Status;
        elevator.MoveElevator(destinationFloor);
        _logger.LogElevatorStatusChange(elevator, oldStatus, elevator.Status);

        var fromFloor = elevator.CurrentFloor;
        while (elevator.IsMoving)
        {
            await Task.Delay(1000); //Delay movement simulator
            elevator.UpdateElevatorPosition();
        }

        _logger.LogElevatorMovement(elevator, fromFloor, destinationFloor);

        await OnElevatorArrivedAsync(elevator);

        oldStatus = elevator.Status;
        _logger.LogElevatorStatusChange(elevator, oldStatus, elevator.Status);
    }

    public async Task OnElevatorArrivedAsync(Elevator elevator)
    {
        await UnloadPassengersAsync(elevator);
        await LoadPassengersAsync(elevator, elevator.CurrentFloor);
    }

    private async Task UnloadPassengersAsync(Elevator elevator)
    {
        var passengersToUnload = elevator.Passengers.Where(p => p.DestinationFloor == elevator.CurrentFloor).ToList();
        foreach (var passenger in passengersToUnload)
        {
            elevator.RemovePassenger(passenger);
            _logger.LogPassengerExit(elevator, passenger);
        }
        await Task.Delay(1000); // Unloading time
    }
}