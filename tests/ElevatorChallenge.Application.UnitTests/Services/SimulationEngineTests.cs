using System.Reflection;

namespace ElevatorChallenge.Application.UnitTests.Services;

public class SimulationEngineTests
{
    private readonly Mock<Building> _mockBuilding;
    private readonly Mock<IElevatorService> _mockElevatorService;
    private readonly Mock<IConsoleUserInterface> _mockUserInterface;
    private readonly SimulationEngine _simulationEngine;

    public SimulationEngineTests()
    {
        _mockBuilding = new Mock<Building>(10, 3, 8, 1000);
        _mockElevatorService = new Mock<IElevatorService>();
        _mockUserInterface = new Mock<IConsoleUserInterface>();
        _simulationEngine = new SimulationEngine(_mockBuilding.Object, _mockElevatorService.Object, _mockUserInterface.Object);
    }

    [Fact]
    public async Task RunSimulation_ShouldInitializeUIAndStartLoops()
    {
        _mockUserInterface.Setup(ui => ui.GetUserInputAsync()).ReturnsAsync("quit");

        await _simulationEngine.RunSimulation();

        _mockUserInterface.Verify(ui => ui.Initialize(), Times.Once);
        _mockUserInterface.Verify(ui => ui.DisplayElevatorStatus(_mockBuilding.Object), Times.AtLeastOnce);
    }

    [Fact]
    public async Task HandleUserInputAsync_CallElevator_ShouldDispatchElevator()
    {
        _mockUserInterface.SetupSequence(ui => ui.GetUserInputAsync())
                          .ReturnsAsync("call 5 up")
                          .ReturnsAsync("quit");

        await _simulationEngine.RunSimulation();

        _mockElevatorService.Verify(es => es.DispatchElevatorAsync(5, Direction.Up), Times.Once);
    }

    [Fact]
    public async Task HandleUserInputAsync_MoveElevator_ShouldMoveElevator()
    {
        _mockUserInterface.SetupSequence(ui => ui.GetUserInputAsync())
                          .ReturnsAsync("move 1 5")
                          .ReturnsAsync("quit");

        await _simulationEngine.RunSimulation();

        var movedElevator = _mockBuilding.Object.Elevators.FirstOrDefault(e => e.Id == 1);
        Assert.NotNull(movedElevator);
        _mockElevatorService.Verify(es => es.MoveElevatorAsync(It.Is<Elevator>(e => e.Id == 1), 5), Times.Once);
    }

    [Fact]
    public async Task HandleUserInputAsync_AddPassenger_ShouldAddPassengerToFloor()
    {
        _mockUserInterface.SetupSequence(ui => ui.GetUserInputAsync())
                          .ReturnsAsync("add 3 5")
                          .ReturnsAsync("quit");

        await _simulationEngine.RunSimulation();

        var floor = _mockBuilding.Object.GetFloor(3);
        Assert.Single(floor.WaitingPassengers);
        var addedPassenger = floor.WaitingPassengers.First();
        Assert.Equal(5, addedPassenger.DestinationFloor);
    }

    [Fact]
    public void UpdateElevatorPositions_ShouldUpdateMovingElevators()
    {
        var elevator1 = _mockBuilding.Object.Elevators[0];
        var elevator2 = _mockBuilding.Object.Elevators[1];

        // Set elevator1 to be moving upwards
        elevator1.MoveElevator(5);  // Move destination floor to 5

        // Set elevator2 to be stationary
        elevator2.MoveElevator(elevator2.CurrentFloor);  // Move to current floor (stationary)

        int initialFloor1 = elevator1.CurrentFloor;
        int initialFloor2 = elevator2.CurrentFloor;

        // Directly invoke the UpdateElevatorPositions method using reflection
        MethodInfo updateMethod = typeof(SimulationEngine).GetMethod("UpdateElevatorPositions", BindingFlags.NonPublic | BindingFlags.Instance);
        updateMethod.Invoke(_simulationEngine, null);

        Assert.True(elevator1.CurrentFloor > initialFloor1, $"Moving elevator should have updated its position. Initial: {initialFloor1}, Current: {elevator1.CurrentFloor}");
        //Assert.Equal(initialFloor2, elevator2.CurrentFloor, "Stationary elevator should not have moved");
        Assert.True((bool)typeof(Elevator).GetProperty("IsMoving").GetValue(elevator1));
        Assert.False((bool)typeof(Elevator).GetProperty("IsMoving").GetValue(elevator2));
    }
}
