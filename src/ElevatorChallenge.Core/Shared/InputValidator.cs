namespace ElevatorChallenge.Core.Shared;

public static class InputValidator
{
    public static void ValidateFloorNumber(int floorNumber, int maxFloors)
    {
        if (floorNumber < 1 || floorNumber > maxFloors)
        {
            throw new ArgumentOutOfRangeException(nameof(floorNumber), $"Floor number must be between 1 and {maxFloors}");
        }
    }

    public static void ValidateElevatorId(int elevatorId, int maxElevators)
    {
        if (elevatorId < 1 || elevatorId > maxElevators)
        {
            throw new ArgumentOutOfRangeException(nameof(elevatorId), $"Elevator ID must be between 1 and {maxElevators}");
        }
    }

    public static void ValidateDirection(Direction direction)
    {
        if (!Enum.IsDefined(typeof(Direction), direction))
        {
            throw new ArgumentException("Invalid direction", nameof(direction));
        }
    }
}
