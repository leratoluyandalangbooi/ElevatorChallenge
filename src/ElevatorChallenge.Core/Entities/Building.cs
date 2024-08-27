namespace ElevatorChallenge.Core.Entities;

public class Building
{
    public List<Floor> Floors { get; }
    public List<Elevator> Elevators { get; }

    public Building(int numberOfFloors, int numberOfElevators, int elevatorCapacity, int elevatorWeightLimit)
    {
        Floors = Enumerable.Range(1, numberOfFloors)
                           .Select(floorNumber => new Floor(floorNumber))
                           .ToList();

        Elevators = Enumerable.Range(1, numberOfElevators)
                              .Select(elevatorId => new Elevator(elevatorId, elevatorCapacity, elevatorWeightLimit))
                              .ToList();
    }

    public Floor GetFloor(int floorNumber)
    {
        return Floors.FirstOrDefault(f => f.FloorNumber == floorNumber);
    }

    public Elevator GetElevator(int elevatorId)
    {
        return Elevators.FirstOrDefault(e => e.Id == elevatorId);
    }

    private void AddPassengerToFloor(int floorNumber, Passenger passenger)
    {
        var floor = GetFloor(floorNumber);
        floor.AddWaitingPassnger(passenger);
    }
}
