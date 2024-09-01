using ElevatorChallenge.Core.Enums;
using Microsoft.Extensions.Logging;

namespace ElevatorChallenge.Infrastructure.Logging;

public class ElevatorLogger : IElevatorLogger
{
    private readonly ILogger<ElevatorLogger> _logger;

    public ElevatorLogger(ILogger<ElevatorLogger> logger)
    {
        _logger = logger;
    }

    public void LogElevatorMovement(Elevator elevator, int fromFloor, int toFloor)
    {
        _logger.LogInformation($"Elevator {elevator.Id} moved from floor {fromFloor} to {toFloor}");
    }

    public void LogPassengerBoarding(Elevator elevator, Passenger passenger)
    {
        _logger.LogInformation($"Passenger {passenger.Id} boarded elevator {elevator.Id} on floor {elevator.CurrentFloor}. Destination: {passenger.DestinationFloor}");
    }

    public void LogPassengerExit(Elevator elevator, Passenger passenger)
    {
        _logger.LogInformation($"Passenger {passenger.Id} exited elevator {elevator.Id} on floor {elevator.CurrentFloor}");
    }

    public void LogElevatorStatusChange(Elevator elevator, ElevatorStatus oldStatus, ElevatorStatus newStatus)
    {
        _logger.LogInformation($"Elevator {elevator.Id} status changed from {oldStatus} to {newStatus}");
    }

    public void LogError(string message, Exception ex = null)
    {
        if (ex != null)
        {
            _logger.LogError(ex, message);
        }
        else
        {
            _logger.LogError(message);
        }
    }
}
