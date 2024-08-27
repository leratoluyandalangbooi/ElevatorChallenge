namespace ElevatorChallenge.Core.Interfaces;

public interface IElevatorService
{
    Task MoveElevatorAsync(Elevator elevator, int destinationFloor);
}