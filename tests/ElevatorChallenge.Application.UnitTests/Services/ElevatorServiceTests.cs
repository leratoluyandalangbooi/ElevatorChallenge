namespace ElevatorChallenge.Application.UnitTests.Services;

public class ElevatorServiceTests
{
    private readonly Mock<Building> _mockBuilding;
    private readonly Mock<INearestAvailableElevatorStrategy> _mockStrategy;
    private readonly Mock<IElevatorLogger> _mockLogger;
    private readonly ElevatorService _elevatorService;

    public ElevatorServiceTests()
    {
        _mockBuilding = new Mock<Building>(10, 3, 8, 1000);
        _mockStrategy = new Mock<INearestAvailableElevatorStrategy>();
        _mockLogger = new Mock<IElevatorLogger>();
        _elevatorService = new ElevatorService(_mockBuilding.Object, _mockStrategy.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task DispatchElevatorAsync_ShouldCallSelectElevatorAndMoveElevator()
    {
        var elevator = new Elevator(1, 10, 1000);
        elevator.MoveElevator(1); // Set initial floor to 1
        _mockStrategy.Setup(s => s.SelectElevator((List<Elevator>)It.IsAny<IEnumerable<Elevator>>(), It.IsAny<int>(), It.IsAny<Direction>()))
                     .Returns(elevator);

        await _elevatorService.DispatchElevatorAsync(5, Direction.Up);

        _mockStrategy.Verify(s => s.SelectElevator((List<Elevator>)It.IsAny<IEnumerable<Elevator>>(), 5, Direction.Up), Times.Once);
        Assert.Equal(5, elevator.DestinationFloor);
        _mockLogger.Verify(l => l.LogElevatorStatusChange(elevator, It.IsAny<ElevatorStatus>(), It.IsAny<ElevatorStatus>()), Times.AtLeastOnce);
        _mockLogger.Verify(l => l.LogElevatorMovement(elevator, 1, 5), Times.Once);
    }

    [Fact]
    public async Task MoveElevatorAsync_ShouldUpdateElevatorPositionUntilDestinationReached()
    {
        var elevator = new Elevator(1, 10, 1000);
        elevator.MoveElevator(1); // Set initial floor to 1

        await _elevatorService.MoveElevatorAsync(elevator, 5);

        Assert.Equal(5, elevator.CurrentFloor);
        Assert.False(elevator.IsMoving);
        _mockLogger.Verify(l => l.LogElevatorStatusChange(elevator, It.IsAny<ElevatorStatus>(), It.IsAny<ElevatorStatus>()), Times.AtLeastOnce);
        _mockLogger.Verify(l => l.LogElevatorMovement(elevator, 1, 5), Times.Once);
    }

        [Fact]
    public async Task OnElevatorArrivedAsync_ShouldUnloadAndLoadPassengers()
    {
        var elevator = new Elevator(1, 10, 1000);
        elevator.MoveElevator(5); // Set current floor to 5
        var passengerToUnload = new Passenger(1, 5, 70);
        elevator.AddPassenger(passengerToUnload);

        while (elevator.IsMoving)
        {
            elevator.UpdateElevatorPosition();
        }

        await _elevatorService.OnElevatorArrivedAsync(elevator);

        Assert.DoesNotContain(passengerToUnload, elevator.Passengers);
        _mockLogger.Verify(l => l.LogPassengerExit(elevator, passengerToUnload), Times.Once);
    }

    [Fact]
    public async Task DispatchElevatorAsync_ShouldLogErrorWhenExceptionOccurs()
    {
        _mockStrategy.Setup(s => s.SelectElevator((List<Elevator>)It.IsAny<IEnumerable<Elevator>>(), It.IsAny<int>(), It.IsAny<Direction>()))
                     .Throws(new Exception("Test exception"));

        await Assert.ThrowsAsync<Exception>(() => _elevatorService.DispatchElevatorAsync(5, Direction.Up));
        _mockLogger.Verify(l => l.LogError("Error dispatching elevator", It.IsAny<Exception>()), Times.Once);
    }
}