namespace ElevatorChallenge.Core.Entities;

public class Building
{
    public List<Floor> Floors { get; }
    public List<Elevator> Elevators { get; }

    public Building()
    {
        
    }

    public Floor GetFloor(int floorNumber)
    {
        return Floors.FirstOrDefault(f => f.FloorNumber == floorNumber);
    }

    public Elevator GetElevator(int elevatorId)
    {
        return Elevators.FirstOrDefault(e => e.Id == elevatorId);
    }
}
