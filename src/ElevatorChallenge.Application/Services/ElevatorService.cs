namespace ElevatorChallenge.Application.Services;

public class ElevatorService : IElevatorService
{
    private readonly Building _building;

    public ElevatorService(Building building)
    {
        _building = building;
    }

    public async Task MoveElevatorAsync(Elevator elevator, int destinationFloor)
    {
        elevator.Status = ElevatorStatus.Moving;

        while (elevator.CurrentFloor != destinationFloor)
        {
            await Task.Delay(1000); //Delay movement simulator
            elevator.CurrentFloor += elevator.Direction == Direction.Up ? 1 : -1;
        }

        elevator.Status = ElevatorStatus.Idle;
    }
}