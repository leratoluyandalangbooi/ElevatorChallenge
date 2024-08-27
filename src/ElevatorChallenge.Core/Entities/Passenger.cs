
namespace ElevatorChallenge.Core.Entities;

public class Passenger
{
    public int Id { get; }
    public int DestnationFloor { get; }
    public int Weight { get; }

    public Passenger(int id, int destinationFloor, int weight)
    {
        Id = id;
        DestnationFloor = destinationFloor;
        Weight = weight;
    }
}
