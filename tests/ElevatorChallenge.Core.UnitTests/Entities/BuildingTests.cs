namespace ElevatorChallenge.Core.UnitTests.Entities;

public class BuildingTests
{
    [Fact]
    public void Building_Constructor_ShouldCreateFloorsAndElevators()
    {
        var building = new Building(10, 3, 10, 1000);

        Assert.Equal(10, building.Floors.Count);
        Assert.Equal(3, building.Elevators.Count);
        Assert.All(building.Elevators, elevator => Assert.Equal(10, elevator.Capacity));
        Assert.All(building.Elevators, elevator => Assert.Equal(1000, elevator.WeightLimit));
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(0, 1)]
    [InlineData(10, 0)]
    public void Building_Constructor_ShouldThrowArgumentException(int floors, int elevators)
    {
        Assert.Throws<ArgumentException>(() => new Building(floors, elevators, 10, 1000));
    }

    [Fact]
    public void GetFloor_ShouldReturnCorrectFloor()
    {
        var building = new Building(10, 3, 10, 1000);

        var floor = building.GetFloor(5);

        Assert.Equal(5, floor.FloorNumber);
    }

    [Fact]
    public void GetFloor_ShouldThrowArgumentException_WhenFloorDoesNotExist()
    {
        var building = new Building(10, 3, 10, 1000);

        Assert.Throws<ArgumentException>(() => building.GetFloor(11));
    }
}