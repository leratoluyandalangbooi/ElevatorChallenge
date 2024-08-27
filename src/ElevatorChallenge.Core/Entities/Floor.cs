namespace ElevatorChallenge.Core.Entities;

public class Floor
{
    public int FloorNumber { get; }
    public Queue<Passenger> WaitingPassengers { get; }
    public Dictionary<Direction, bool> ElevatorCallButtons { get; }

    public Floor(int floorNumber)
    {
        FloorNumber = floorNumber;
        WaitingPassengers = new Queue<Passenger>();
        ElevatorCallButtons = new Dictionary<Direction, bool>
        {
            { Direction.Up, false },
            { Direction.Down, false }
        };
    }

    public void AddWaitingPassnger(Passenger passenger)
    {
        WaitingPassengers.Enqueue(passenger);
        UpdateElevetorCallButton(passenger.DestnationFloor);
    }

    private void UpdateElevetorCallButton(int destnationFloor)
    {
        var direction = destnationFloor > FloorNumber ? Direction.Up : Direction.Down;
        ElevatorCallButtons[direction] = true;
    }

    public Passenger? RemoveWaitingPassenger()
    {
        if (WaitingPassengers.Count == 0) return null;

        var passenger = WaitingPassengers.Dequeue();
        if (WaitingPassengers.Count == 0) ResetElevatorCallButton();
        return passenger;
    }

    private void ResetElevatorCallButton()
    {
        ElevatorCallButtons[Direction.Up] = false;
        ElevatorCallButtons[Direction.Down] = false;
    }

    public int WaitingPassengersCount => WaitingPassengers.Count;
}
