namespace ElevatorChallenge.Application.Services;

public class ElevatorService : IElevatorService
{
    private readonly Building _building;
    private readonly INearestAvailableElevatorStrategy _elevatorDispatchStrategy;

    public ElevatorService(Building building, INearestAvailableElevatorStrategy elevatorDispatchStrategy)
    {
        _building = building;
        _elevatorDispatchStrategy = elevatorDispatchStrategy;
    }

    public async Task DispatchElevatorAsync(int requestedFloor, Direction direction)
    {
        var elevator = _elevatorDispatchStrategy.SelectElevator(_building.Elevators, requestedFloor, direction);
        await MoveElevatorAsync(elevator, requestedFloor);
        await LoadPassengersAsync(elevator, requestedFloor);
    }

    private async Task LoadPassengersAsync(Elevator elevator, int requestedFloor)
    {
        var floor = _building.GetFloor(requestedFloor);
        var passengerQueued = floor.WaitingPassengersCount > 0 && elevator.CanAddPassenger(floor.WaitingPassengers.Peek());
        while (passengerQueued)
        {
            var passenger = floor.RemoveWaitingPassenger();
            elevator.AddPassenger(passenger!);
        }
        await Task.Delay(1000); //Loading time
    }

    public async Task MoveElevatorAsync(Elevator elevator, int destinationFloor)
    {
        elevator.MoveElevator(destinationFloor);

        while (elevator.IsMoving)
        {
            await Task.Delay(1000); //Delay movement simulator
            elevator.UpdateElevatorPosition();
        }

        await OnLoadOrOffloadPassengersAsync(elevator);
    }

    private async Task OnLoadOrOffloadPassengersAsync(Elevator elevator)
    {
        // Offload passengers
        var passengersToUnload = elevator.Passengers.Where(p => p.DestinationFloor == elevator.CurrentFloor).ToList();
        foreach (var passenger in passengersToUnload)
        {
            elevator.RemovePassenger(passenger);
        }

        // Onload passengers
        await LoadPassengersAsync(elevator, elevator.CurrentFloor);
    }
}