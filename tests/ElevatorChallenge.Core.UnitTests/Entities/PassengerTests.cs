namespace ElevatorChallenge.Core.UnitTests.Entities;

public class PassengerTests
{
    [Fact]
    public void Passenger_Constructor_ShouldSetProperties()
    {
        var passenger = new Passenger(1, 5, 70);

        Assert.Equal(1, passenger.Id);
        Assert.Equal(5, passenger.DestinationFloor);
        Assert.Equal(70, passenger.Weight);
    }
}
