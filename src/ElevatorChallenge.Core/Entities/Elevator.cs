namespace ElevatorChallenge.Core.Entities;

public class Elevator
{
    public int Id { get; private set; }
    public int CurrentFloor { get; set; }
    public Direction Direction { get; set; }
    public ElevatorStatus Status { get; set; }
    public List<Passenger> Passengers { get; }
    public int Capacity { get; }
    public int WeightLimit { get; }
    public int CurrentWeight => Passengers.Sum(p => p.Weight);
    public int DestinationFloor { get; private set; }
    public bool IsMoving => CurrentFloor != DestinationFloor;

    public Elevator(int id, int capacity, int weightLimit)
    {
        Id = id;
        CurrentFloor = 1;
        Direction = Direction.Stationary;
        Status = ElevatorStatus.Idle;
        Passengers = new List<Passenger>();
        Capacity = capacity;
        WeightLimit = weightLimit;
        DestinationFloor = 1;
    }

    public void MoveElevator(int destinationFloor)
    {
        if (destinationFloor == CurrentFloor)
        {
            SetIdle();
        }
        else
        {
            Direction = destinationFloor > CurrentFloor ? Direction.Up : Direction.Down;
            Status = ElevatorStatus.Moving;
            DestinationFloor = destinationFloor;
        }
    }

    public void UpdateElevatorPosition()
    {
        if (CurrentFloor < DestinationFloor)
        {
            CurrentFloor++;
        }
        else if (CurrentFloor > DestinationFloor)
        {
            CurrentFloor--;
        }

        if (CurrentFloor == DestinationFloor)
        {
            SetIdle();
        }
    }

    public void AddPassenger(Passenger passenger)
    {
        if (!CanAddPassenger(passenger))
        {
            throw new ElevatorException("Cannot add passenger: Elevator capacity or weight limit reached");
        }

        Passengers.Add(passenger); 
    }
    
    public void RemovePassenger(Passenger passenger)
    {
        Passengers.Remove(passenger);
    }

    public bool CanAddPassenger(Passenger passenger)
    {
        return Passengers.Count < Capacity && CurrentWeight + passenger.Weight < WeightLimit;
    }

    public void SetIdle()
    {
        Status = ElevatorStatus.Idle;
        Direction = Direction.Stationary;
    }
}