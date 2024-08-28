namespace ElevatorChallenge.Core.Interfaces;

public interface INearestAvailableElevatorStrategy
{
    Elevator SelectElevator(List<Elevator> elevators, int requestedFloor, Direction direction);
}
