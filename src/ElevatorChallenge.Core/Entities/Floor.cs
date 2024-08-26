namespace ElevatorChallenge.Core.Entities;

public class Floor
{
    public int FloorNumber { get; }
    public Queue<Passenger> PassengersQueue { get; }
    public Dictionary<Direction, bool> ElevatorCallButtons { get; }

    public Floor(int floorNumber)
    {
        FloorNumber = floorNumber;
        PassengersQueue = new Queue<Passenger>();
        ElevatorCallButtons = new Dictionary<Direction, bool>
        {
            { Direction.Up, false },
            { Direction.Down, false }
        };
    }

}
