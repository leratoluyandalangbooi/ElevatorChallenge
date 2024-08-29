namespace ElevatorChallenge.Core.UnitTests.Entities;

public class ElevatorTests
{
    [Fact]
    public void Constructor_InitializesPropertiesCorrectly()
    {
        var elevator = new Elevator(1, 10, 1000);

        Assert.Equal(1, elevator.Id);
        Assert.Equal(10, elevator.Capacity);
        Assert.Equal(1000, elevator.WeightLimit);
        Assert.Equal(1, elevator.CurrentFloor);
        Assert.Equal(1, elevator.DestinationFloor);
        Assert.Equal(Direction.Stationary, elevator.Direction);
        Assert.Equal(ElevatorStatus.Idle, elevator.Status);
        Assert.Empty(elevator.Passengers);
        Assert.False(elevator.IsMoving);
    }

    [Fact]
    public void CanAddPassenger_ShouldReturnTrue_WhenBelowCapacityAndWeightLimit()
    {
        var elevator = new Elevator(1, 5, 500);
        var passenger = new Passenger(1, 5, 70);

        var canAdd = elevator.CanAddPassenger(passenger);

        Assert.True(canAdd);
    }

    [Fact]
    public void CanAddPassenger_ShouldReturnFalse_WhenAtCapacity()
    {
        var elevator = new Elevator(1, 2, 500);
        elevator.AddPassenger(new Passenger(1, 5, 70));
        elevator.AddPassenger(new Passenger(2, 5, 70));
        var newPassenger = new Passenger(3, 5, 70);

        var canAdd = elevator.CanAddPassenger(newPassenger);

        Assert.False(canAdd);
    }

    [Fact]
    public void CanAddPassenger_ShouldReturnFalse_WhenExceedingWeightLimit()
    {
        var elevator = new Elevator(1, 5, 200);
        elevator.AddPassenger(new Passenger(1, 5, 150));
        var heavyPassenger = new Passenger(2, 5, 100);

        var canAdd = elevator.CanAddPassenger(heavyPassenger);

        Assert.False(canAdd);
    }

    [Fact]
    public void AddPassenger_ShouldAddPassengerSuccessfully()
    {
        var elevator = new Elevator(1, 5, 500);
        var passenger = new Passenger(1, 5, 70);

        elevator.AddPassenger(passenger);

        Assert.Single(elevator.Passengers);
        Assert.Equal(passenger, elevator.Passengers[0]);
    }

    [Fact]
    public void AddPassenger_ShouldThrowException_WhenAtCapacity()
    {
        var elevator = new Elevator(1, 2, 500);
        elevator.AddPassenger(new Passenger(1, 5, 70));
        elevator.AddPassenger(new Passenger(2, 5, 70));

        Assert.Throws<ElevatorException>(() => elevator.AddPassenger(new Passenger(3, 5, 70)));
    }

    [Fact]
    public void RemovePassenger_ShouldRemovePassengerSuccessfully()
    {
        var elevator = new Elevator(1, 5, 500);
        var passenger = new Passenger(1, 5, 70);
        elevator.AddPassenger(passenger);

        elevator.RemovePassenger(passenger);

        Assert.Empty(elevator.Passengers);
        Assert.Equal(0, elevator.CurrentWeight);
    }

    [Fact]
    public void RemovePassenger_ShouldDoNothingWhenPassengerNotInElevator()
    {
        var elevator = new Elevator(1, 5, 500);
        var passenger1 = new Passenger(1, 5, 70);
        var passenger2 = new Passenger(2, 5, 80);
        elevator.AddPassenger(passenger1);

        elevator.RemovePassenger(passenger2);

        Assert.Single(elevator.Passengers);
        Assert.Equal(70, elevator.CurrentWeight);
    }

    [Theory]
    [InlineData(1, 5, Direction.Up, ElevatorStatus.Moving)]
    [InlineData(5, 1, Direction.Down, ElevatorStatus.Moving)]
    [InlineData(3, 3, Direction.Stationary, ElevatorStatus.Idle)]
    public void MoveElevator_ShouldDirectionAndStatus(int currentFloor, int destinationFloor, Direction expectedDirection, ElevatorStatus expectedStatus)
    {
        var elevator = new Elevator(1, 10, 1000);
        elevator.MoveElevator(currentFloor);

        while (elevator.CurrentFloor != elevator.DestinationFloor)
        {
            elevator.UpdateElevatorPosition();
        }

        elevator.MoveElevator(destinationFloor);

        Assert.Equal(expectedDirection, elevator.Direction);
        Assert.Equal(expectedStatus, elevator.Status);
        Assert.Equal(destinationFloor, elevator.DestinationFloor);
        Assert.Equal(currentFloor != destinationFloor, elevator.IsMoving);
    }

    

    [Fact]
    public void MoveElevator_ShouldUpdateStatusToIdleWhenReachingDestination()
    {
        var elevator = new Elevator(1, 5, 500);
        elevator.MoveElevator(5); // Move to floor 5

        while (elevator.CurrentFloor != elevator.DestinationFloor)
        {
            elevator.UpdateElevatorPosition();
        }

        Assert.Equal(Direction.Stationary, elevator.Direction);
        Assert.Equal(5, elevator.CurrentFloor);
        Assert.Equal(5, elevator.DestinationFloor);
    }

    [Fact]
    public void MoveElevator_ShouldNotChangeStateWhenAlreadyAtDestination()
    {
        var elevator = new Elevator(1, 5, 500);

        elevator.MoveElevator(1); // Already at floor 1

        Assert.Equal(ElevatorStatus.Idle, elevator.Status);
        Assert.Equal(Direction.Stationary, elevator.Direction);
        Assert.Equal(1, elevator.CurrentFloor);
        Assert.Equal(1, elevator.DestinationFloor);
    }

    [Fact]
    public void UpdateElevatorPosition_ShouldMoveElevatorTowardsDestination()
    {
        var elevator = new Elevator(1, 10, 1000);
        elevator.MoveElevator(5);

        elevator.UpdateElevatorPosition();

        Assert.Equal(2, elevator.CurrentFloor);
        Assert.True(elevator.IsMoving);
        Assert.Equal(ElevatorStatus.Moving, elevator.Status);
    }

    [Fact]
    public void UpdateElevatorPosition_ShouldStopAtDestination()
    {
        var elevator = new Elevator(1, 10, 1000);
        elevator.MoveElevator(2);

        elevator.UpdateElevatorPosition();

        Assert.Equal(2, elevator.CurrentFloor);
        Assert.False(elevator.IsMoving);
        Assert.Equal(ElevatorStatus.Idle, elevator.Status);
        Assert.Equal(Direction.Stationary, elevator.Direction);
    }

    [Fact]
    public void CurrentWeight_ShouldCalculateCorrectly()
    {
        var elevator = new Elevator(1, 5, 500);
        elevator.AddPassenger(new Passenger(1, 5, 70));
        elevator.AddPassenger(new Passenger(2, 5, 80));

        var currentWeight = elevator.CurrentWeight;

        Assert.Equal(150, currentWeight);
    }
}