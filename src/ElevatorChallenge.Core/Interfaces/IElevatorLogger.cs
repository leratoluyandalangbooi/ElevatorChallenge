namespace ElevatorChallenge.Core.Interfaces;

public interface IElevatorLogger
{
    void LogElevatorMovement(Elevator elevator, int fromFloor, int toFloor);
    void LogPassengerBoarding(Elevator elevator, Passenger passenger);
    void LogPassengerExit(Elevator elevator, Passenger passenger);
    void LogElevatorStatusChange(Elevator elevator, ElevatorStatus oldStatus, ElevatorStatus newStatus);
    void LogError(string message, Exception ex = null);
}
