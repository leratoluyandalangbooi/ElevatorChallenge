namespace ElevatorChallenge.Infrastructure.UnitTests.UserInterface;

public class ConsoleUserInterfaceTests
{
    private readonly Mock<IConsoleWrapper> _mockConsole;
    private readonly ConsoleUserInterface _userInterface;

    public ConsoleUserInterfaceTests()
    {
        _mockConsole = new Mock<IConsoleWrapper>();
        _userInterface = new ConsoleUserInterface(_mockConsole.Object);
    }

    [Fact]
    public void DisplayElevatorStatus_ShouldDisplayCorrectInformation()
    {
        var building = CreateMockBuilding();
        var displayedContent = "";
        _mockConsole.Setup(c => c.Write(It.IsAny<string>()))
                    .Callback<string>(s => displayedContent += s);

        _userInterface.DisplayElevatorStatus(building);

        Assert.Contains("ELEVATOR SIMULATION ENGINE", displayedContent);
        Assert.Contains("Elevator Status:", displayedContent);
        Assert.Contains("Elevator 1/2: Floor 1/10 | Idle | Stationary | Passenger:0/8", displayedContent);
        Assert.Contains("Floor Requests:", displayedContent);
        Assert.Contains("Floor 5: ▲ ▼ Waiting: 2", displayedContent);
        Assert.Contains("Available Commands:", displayedContent);
    }

    [Fact]
    public async Task GetUserInputAsync_ShouldReturnUserInput()
    {
        var expectedInput = "test input";
        _mockConsole.Setup(c => c.ReadLine()).Returns(expectedInput);

        var result = await _userInterface.GetUserInputAsync();

        Assert.Equal(expectedInput, result);
    }

    [Fact]
    public void DisplayMessage_ShouldAddMessageToQueue()
    {
        var message = "Test message";
        var displayedContent = "";
        _mockConsole.Setup(c => c.Write(It.IsAny<string>()))
                    .Callback<string>(s => displayedContent += s);

        _userInterface.DisplayMessage(message);
        _userInterface.DisplayElevatorStatus(CreateMockBuilding());

        Assert.Contains(message, displayedContent);
    }

    [Fact]
    public void Initialize_ShouldClearConsole()
    {
        _userInterface.Initialize();

        _mockConsole.Verify(c => c.Clear(), Times.Once);
    }

    private Building CreateMockBuilding()
    {
        var building = new Building(10, 2, 8, 1000);
        var floor5 = building.GetFloor(5);
        floor5.ElevatorCallButtons[Direction.Up] = true;
        floor5.ElevatorCallButtons[Direction.Down] = true;
        floor5.AddWaitingPassenger(new Passenger(1, 10, 70));
        floor5.AddWaitingPassenger(new Passenger(2, 1, 70));
        return building;
    }
}