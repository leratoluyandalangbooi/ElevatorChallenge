namespace ElevatorChallenge.Application.UnitTests.Services;

public class ElevatorServiceTests
{
    private readonly Mock<Building> _mockBuilding;
    private readonly Mock<INearestAvailableElevatorStrategy> _mockStrategy;
    private readonly ElevatorService _elevatorService;

    public ElevatorServiceTests()
    {
        _mockBuilding = new Mock<Building>(10, 3, 8, 1000);
        _mockStrategy = new Mock<INearestAvailableElevatorStrategy>();
        _elevatorService = new ElevatorService(_mockBuilding.Object, _mockStrategy.Object);
    }

    [Fact]
    public async Task DispatchElevatorAsync_ShouldCallSelectElevatorAndMoveElevator()
    {
        var elevator = new Elevator(1, 10, 1000);
        _mockStrategy.Setup(s => s.SelectElevator((List<Elevator>)It.IsAny<IEnumerable<Elevator>>(), It.IsAny<int>(), It.IsAny<Direction>()))
                     .Returns(elevator);

        await _elevatorService.DispatchElevatorAsync(5, Direction.Up);

        _mockStrategy.Verify(s => s.SelectElevator((List<Elevator>)It.IsAny<IEnumerable<Elevator>>(), 5, Direction.Up), Times.Once);
        Assert.Equal(5, elevator.DestinationFloor);
    }

    [Fact]
    public async Task MoveElevatorAsync_ShouldUpdateElevatorPositionUntilDestinationReached()
    {
        var elevator = new Elevator(1, 10, 1000);
        elevator.MoveElevator(5);  // Set destination to 5

        await _elevatorService.MoveElevatorAsync(elevator, 5);

        Assert.Equal(5, elevator.CurrentFloor);
        Assert.False(elevator.IsMoving);
    }

    [Fact]
    public async Task OnElevatorArrivedAsync_ShouldUnloadAndLoadPassengers()
    {
        var elevator = new Elevator(1, 10, 1000);
        const int destinationFloor = 5;
        elevator.MoveElevator(destinationFloor);  // Move to floor 5
        var passengerToUnload = new Passenger(1, destinationFloor, 70);
        elevator.AddPassenger(passengerToUnload);  // Add a passenger going to floor 5

#if DEBUG
        Console.WriteLine("Initial state:");
        Console.WriteLine($"Elevator {elevator.Id} current floor: {elevator.CurrentFloor}");
        Console.WriteLine($"Elevator {elevator.Id} destination floor: {elevator.DestinationFloor}");
        Console.WriteLine($"Passenger destination floor: {passengerToUnload.DestinationFloor}");
#endif

        // Ensure the elevator has reached the destination floor
        while (elevator.IsMoving)
        {
            elevator.UpdateElevatorPosition();
        }

        Console.WriteLine("\nBefore OnElevatorArrivedAsync:");
        Console.WriteLine($"Elevator {elevator.Id} current floor: {elevator.CurrentFloor}");

        await _elevatorService.OnElevatorArrivedAsync(elevator);

#if DEBUG
        Console.WriteLine("\nAfter OnElevatorArrivedAsync:");
        Console.WriteLine($"Elevator {elevator.Id} current floor: {elevator.CurrentFloor}");
        Console.WriteLine($"Elevator {elevator.Id} passengers: {string.Join(", ", elevator.Passengers.Select(p => p.ToString()))}");
#endif

        Assert.DoesNotContain(passengerToUnload, elevator.Passengers);
        Assert.Empty(elevator.Passengers);
    }
}