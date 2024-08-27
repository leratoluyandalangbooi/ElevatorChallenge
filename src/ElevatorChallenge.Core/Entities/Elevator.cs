using ElevatorChallenge.Core.Exceptions;

namespace ElevatorChallenge.Core.Entities;

public class Elevator
{
    public int Id { get; private set; }
    public int CurrentFloor { get; private set; }
    public Direction Direction { get; private set; }
    public ElevatorStatus Status { get; private set; }
    public List<Passenger> Passengers { get; }
    public int Capacity { get; }
    public int WeightLimit { get; }
    public int CurrentWeight => Passengers.Sum(p => p.Weight);

    public Elevator(int id, int capacity, int weightLimit)
    {
        Id = id;
        CurrentFloor = 1;
        Direction = Direction.Stationary;
        Status = ElevatorStatus.Idle;
        Passengers = new List<Passenger>();
        Capacity = capacity;
        WeightLimit = weightLimit;
    }

    public void SetElevatorStatus(ElevatorStatus elevatorStatus)
    {
        Status = elevatorStatus;
    }

    public void SetElevatorDirection(Direction direction)
    {
        Direction = direction;
    }

    public void MoveElevator(int destinationFloor)
    {
        if (destinationFloor == CurrentFloor)
            return;

        Direction = destinationFloor > CurrentFloor ? Direction.Up : Direction.Down;
        Status = ElevatorStatus.Moving;
        CurrentFloor = destinationFloor;
        Status = ElevatorStatus.Idle;
    }

    public void AddPassenger(Passenger passenger)
    {
        if (Passengers.Count < Capacity && CurrentWeight + passenger.Weight < WeightLimit)
        {
            throw new ElevatorException("Cannot add passenger: Elevator capacity or weight limit reached");
        }

        Passengers.Add(passenger);
    }

    public void RemovePassenger(Passenger passenger)
    {
        Passengers.Remove(passenger);
    }
}