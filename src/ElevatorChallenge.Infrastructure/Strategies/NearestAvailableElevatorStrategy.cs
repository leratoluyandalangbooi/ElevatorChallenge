using ElevatorChallenge.Core.Enums;

namespace ElevatorChallenge.Infrastructure.Strategies;

public class NearestAvailableElevatorStrategy : INearestAvailableElevatorStrategy
{
    public Elevator SelectElevator(List<Elevator> elevators, int requestedFloor, Direction direction)
    {
        var elevator = elevators.Where(x => x.Status != ElevatorStatus.OutOfService)
                        .OrderBy(x => CalculatePriority(x, requestedFloor, direction))
                        .FirstOrDefault();

        if (elevator == null)
            throw new InvalidOperationException("No elevators available");

        return elevator;
    }

    private int CalculatePriority(Elevator elevator, int requestedFloor, Direction direction)
    {
        int distance = Math.Abs(elevator.CurrentFloor - requestedFloor);

        // Prioritize idle elevators or those moving in the same direction
        if (elevator.Status == ElevatorStatus.Idle || elevator.Direction == direction)
        {
            return distance;
        }

        // Lower priority for elevators moving in the opposite direction
        return distance + 1000;
    }
}
