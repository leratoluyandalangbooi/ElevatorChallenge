namespace ElevatorChallenge.Core.Entities;

public class Passenger
{
    public int Id { get; }
    public int DestinationFloor { get; }
    public int Weight { get; }

    public Passenger(int id, int destinationFloor, int weight)
    {
        Id = id;
        DestinationFloor = destinationFloor;
        Weight = weight;
    }
}