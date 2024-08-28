namespace ElevatorChallenge.Core.Interfaces;

public interface IElevatorService
{
    Task DispatchElevatorAsync(int requestedFloor, Direction direction);
    Task MoveElevatorAsync(Elevator elevator, int destinationFloor);
}