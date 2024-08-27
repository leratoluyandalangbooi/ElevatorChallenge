namespace ElevatorChallenge.Core.Entities;

public class Building
{
    public List<Floor> Floors { get; }
    public List<Elevator> Elevators { get; }

    public Building(int numberOfFloors, int numberOfElevators, int elevatorCapacity, int elevatorWeightLimit)
    {
        if (numberOfFloors < 2)
            throw new ArgumentException("Building must have at least 2 floors.", nameof(numberOfFloors));

        if (numberOfElevators < 1)
            throw new ArgumentException("Building must have at least 1 elevator.", nameof(numberOfElevators));

        Floors = Enumerable.Range(1, numberOfFloors)
                           .Select(floorNumber => new Floor(floorNumber))
                           .ToList();

        Elevators = Enumerable.Range(1, numberOfElevators)
                              .Select(elevatorId => new Elevator(elevatorId, elevatorCapacity, elevatorWeightLimit))
                              .ToList();
    }

    public Floor GetFloor(int floorNumber)
    {
        var floor = Floors.FirstOrDefault(f => f.FloorNumber == floorNumber);
        if (floor == null) 
            throw new ArgumentException($"Invalid floor number: {floorNumber}");
        return floor;

    }

    public Elevator GetElevator(int elevatorId)
    {
        var elevator = Elevators.FirstOrDefault(e => e.Id == elevatorId);
        if (elevator == null) 
            throw new ArgumentException($"Invalid elevator ID: {elevatorId}");
        return elevator;
    }

    private void AddPassengerToFloor(int floorNumber, Passenger passenger)
    {
        var floor = GetFloor(floorNumber);
        floor.AddWaitingPassenger(passenger);
    }
}
