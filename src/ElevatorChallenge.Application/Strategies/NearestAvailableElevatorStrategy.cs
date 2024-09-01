namespace ElevatorChallenge.Application.Strategies;

public class NearestAvailableElevatorStrategy : INearestAvailableElevatorStrategy
{
    public Elevator SelectElevator(List<Elevator> elevators, int requestedFloor, Direction direction)
    {
        var elevator = elevators.Where(x => x.Status != ElevatorStatus.OutOfService)
                        .OrderBy(x => CalculatePriority(x, requestedFloor, direction))
                        .ThenBy(x => x.Passengers.Count) // elevator not full or fewer passengers
                        .FirstOrDefault();

        if (elevator == null)
            throw new InvalidOperationException("No elevators available");

        return elevator;
    }

    private double CalculatePriority(Elevator elevator, int requestedFloor, Direction direction)
    {
        double distance = Math.Abs(elevator.CurrentFloor - requestedFloor);
        double priority = distance;
        
        // Splitting prioritisation to idle, same direction and opposite direction as lowest
        if (elevator.Status == ElevatorStatus.Idle)
        {
            priority *= 0.8;
        }
        else if (elevator.Direction == direction)
        {
            if ((direction == Direction.Up && elevator.CurrentFloor <= requestedFloor) ||
                    (direction == Direction.Down && elevator.CurrentFloor >= requestedFloor))
            {
                priority *= 0.9;
            }
            else
            {
                priority *= 1.2;
            }
        }
        else
        {
            priority *= 1.5;
        }

        // Cater for elevator capacity
        double capacityFactor = (double)elevator.Passengers.Count / elevator.Capacity;
        priority += capacityFactor * 5;

        return priority;
    }

    public int GetNextDestination(Elevator elevator, Building building)
    {
        var floorsWithRequests = building.Floors
                                         .Where(f => f.ElevatorCallButtons[elevator.Direction] == true || f.WaitingPassengers.Any())
                                         .ToList();

        if (!floorsWithRequests.Any())
        {
            return elevator.CurrentFloor; // Idle if no request
        }

        // First check if there's any Passengers inside elevator
        var elevatorPassengers = elevator.Passengers
                                         .Select(p => p.DestinationFloor)
                                         .Where(f => (elevator.Direction == Direction.Up && f > elevator.CurrentFloor) ||
                                                     (elevator.Direction == Direction.Down && f < elevator.CurrentFloor))
                                         .OrderBy(f => Math.Abs(f - elevator.CurrentFloor));
        if (elevatorPassengers.Any())
        {
            return elevatorPassengers.First();
        }


        // Then, check if there are any requests in the elevator's current direction
        var sameDirectionFloors = floorsWithRequests.Where(f =>
            (elevator.Direction == Direction.Up && f.FloorNumber > elevator.CurrentFloor) ||
            (elevator.Direction == Direction.Down && f.FloorNumber < elevator.CurrentFloor))
                                   .OrderBy(f => Math.Abs(f.FloorNumber - elevator.CurrentFloor));

        if (sameDirectionFloors.Any())
        {
            return sameDirectionFloors.First().FloorNumber;
        }

        // If no requests in the current direction, change direction then find the nearest request
        elevator.Direction = elevator.Direction == Direction.Up ? Direction.Down : Direction.Up;
        return floorsWithRequests
            .OrderBy(f => Math.Abs(f.FloorNumber - elevator.CurrentFloor))
            .First().FloorNumber;
    }
}
