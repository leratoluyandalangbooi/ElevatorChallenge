
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

    internal static void Add(Passenger passenger)
    {
        throw new NotImplementedException();
    }

    internal static void Remove(Passenger passenger)
    {
        throw new NotImplementedException();
    }
}
