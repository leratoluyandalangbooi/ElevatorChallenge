namespace ElevatorChallenge.Core.UnitTests.Entities;

public class ElevatorTests
{
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
    public void AddPassenger_ShouldThrowException_WhenAtCapacity()
    {
        var elevator = new Elevator(1, 2, 500);
        elevator.AddPassenger(new Passenger(1, 5, 70));
        elevator.AddPassenger(new Passenger(2, 5, 70));

        Assert.Throws<ElevatorException>(() => elevator.AddPassenger(new Passenger(3, 5, 70)));
    }

    [Fact]
    public void Move_ShouldUpdateCurrentFloorAndDirection()
    {
        var elevator = new Elevator(1, 5, 500);

        elevator.MoveElevator(5);

        Assert.Equal(5, elevator.CurrentFloor);
        Assert.Equal(Direction.Up, elevator.Direction);
    }
}