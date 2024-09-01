using ElevatorChallenge.Infrastructure.Logging;
using Microsoft.Extensions.Logging;

namespace ElevatorChallenge.Infrastructure.UnitTests.Logging;

public class ElevatorLoggerTests
{
    private readonly Mock<ILogger<ElevatorLogger>> _mockLogger;
    private readonly ElevatorLogger _elevatorLogger;

    public ElevatorLoggerTests()
    {
        _mockLogger = new Mock<ILogger<ElevatorLogger>>();
        _elevatorLogger = new ElevatorLogger(_mockLogger.Object);
    }

    [Fact]
    public void LogElevatorMovement_ShouldLogCorrectMessage()
    {
        // Arrange
        var elevator = new Elevator(1, 10, 1000);

        // Act
        _elevatorLogger.LogElevatorMovement(elevator, 1, 5);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Elevator 1 moved from floor 1 to 5")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogPassengerBoarding_ShouldLogCorrectMessage()
    {
        // Arrange
        var elevator = new Elevator(1, 10, 1000);
        var passenger = new Passenger(1, 5, 70);

        // Act
        _elevatorLogger.LogPassengerBoarding(elevator, passenger);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Passenger 1 boarded elevator 1 on floor")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogPassengerExit_ShouldLogCorrectMessage()
    {
        // Arrange
        var elevator = new Elevator(1, 10, 1000);
        var passenger = new Passenger(1, 5, 70);

        // Act
        _elevatorLogger.LogPassengerExit(elevator, passenger);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Passenger 1 exited elevator 1 on floor")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogElevatorStatusChange_ShouldLogCorrectMessage()
    {
        // Arrange
        var elevator = new Elevator(1, 10, 1000);

        // Act
        _elevatorLogger.LogElevatorStatusChange(elevator, ElevatorStatus.Idle, ElevatorStatus.Moving);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Elevator 1 status changed from Idle to Moving")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogError_ShouldLogCorrectMessage()
    {
        // Arrange
        var errorMessage = "Test error message";
        var exception = new Exception("Test exception");

        // Act
        _elevatorLogger.LogError(errorMessage, exception);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString().Contains(errorMessage)),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}
