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

    public void AddWaitingPassenger(Passenger passenger)
    {
        WaitingPassengers.Enqueue(passenger);
        UpdateElevetorCallButton(passenger.DestinationFloor);
    }

    private void UpdateElevetorCallButton(int? destinationFloor = null)
    {
        if (WaitingPassengersCount == 0)
        {
            ResetElevatorCallButton();
        }
        else
        {
            if (destinationFloor.HasValue)
            {
                var direction = destinationFloor > FloorNumber ? Direction.Up : Direction.Down;
                ElevatorCallButtons[direction] = true;
            }
            else
            {
                ElevatorCallButtons[Direction.Up] = WaitingPassengers.Any(p => p.DestinationFloor > FloorNumber);
                ElevatorCallButtons[Direction.Down] = WaitingPassengers.Any(p => p.DestinationFloor < FloorNumber);
            }
        }
    }

    public Passenger? RemoveWaitingPassenger()
    {
        if (WaitingPassengers.Count == 0) return null;

        var passenger = WaitingPassengers.Dequeue();
        UpdateElevetorCallButton();
        return passenger;
    }

    private void ResetElevatorCallButton()
    {
        ElevatorCallButtons[Direction.Up] = false;
        ElevatorCallButtons[Direction.Down] = false;
    }

    public int WaitingPassengersCount => WaitingPassengers.Count;
}