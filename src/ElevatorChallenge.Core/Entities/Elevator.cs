namespace ElevatorChallenge.Core.Entities;

public class Elevator
{
    public int Id { get; private set; }
    public int CurrentFloor { get; private set; }
    public Direction Direction {get; private set; }
    public ElevatorStatus Status { get; private set; }
    public List<Passenger> Passengers { get; } = new List<Passenger>();
    public int Capacity { get; }
    public int WeightLimit { get; }
    public int CurrentWeight => Passengers.Sum(p => p.Weight);


}
