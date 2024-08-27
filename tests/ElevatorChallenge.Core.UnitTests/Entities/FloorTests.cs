namespace ElevatorChallenge.Core.UnitTests.Entities;

public class FloorTests
{
    [Fact]
    public void AddWaitingPassenger_ShouldIncreaseWaitingPassengersCount()
    {
        var floor = new Floor(1);
        var passenger = new Passenger(1, 5, 70);

        floor.AddWaitingPassenger(passenger);

        Assert.Equal(1, floor.WaitingPassengersCount);
    }

    [Fact]
    public void RemoveWaitingPassenger_ShouldDecreaseWaitingPassengersCount()
    {
        var floor = new Floor(1);
        floor.AddWaitingPassenger(new Passenger(1, 5, 70));

        var removedPassenger = floor.RemoveWaitingPassenger();

        Assert.Equal(0, floor.WaitingPassengersCount);
        Assert.NotNull(removedPassenger);
    }

    [Fact]
    public void RemoveWaitingPassenger_ShouldReturnNull_WhenNoPassengersWaiting()
    {
        var floor = new Floor(1);

        var removedPassenger = floor.RemoveWaitingPassenger();

        Assert.Null(removedPassenger);
    }
}
