using ElevatorChallenge.Application.Strategies;

namespace ElevatorChallenge.Application.UnitTests.Strategies;

public class NearestAvailableElevatorStrategyTests
{
    private readonly NearestAvailableElevatorStrategy _strategy;
    private const int DefaultElevatorCapacity = 8;
    private const int DefaultElevatorWeightLimit = 1000;

    public NearestAvailableElevatorStrategyTests()
    {
        _strategy = new NearestAvailableElevatorStrategy();
    }

    [Fact]
    public void SelectElevator_ShouldReturnNearestIdleElevator()
    {
        var elevators = new List<Elevator>
        {
            CreateElevator(1, 1, ElevatorStatus.Idle, Direction.Stationary, 0),
            CreateElevator(2, 5, ElevatorStatus.Idle, Direction.Stationary, 0),
            CreateElevator(3, 10, ElevatorStatus.Moving, Direction.Up, 2)
        };

        var selectedElevator = _strategy.SelectElevator(elevators, 3, Direction.Up);

        Assert.Equal(1, selectedElevator.Id);
    }

    [Fact]
    public void SelectElevator_ShouldPreferElevatorMovingInSameDirection()
    {
        var elevators = new List<Elevator>
        {
            CreateElevator(1, 1, ElevatorStatus.Moving, Direction.Up, 0),
            CreateElevator(2, 5, ElevatorStatus.Moving, Direction.Down, 0),
            CreateElevator(3, 7, ElevatorStatus.Moving, Direction.Up, 2)
        };

        var selectedElevator = _strategy.SelectElevator(elevators, 10, Direction.Up);

        Assert.Equal(3, selectedElevator.Id);
        Assert.Equal(Direction.Up, selectedElevator.Direction);
    }

    [Fact]
    public void SelectElevator_ShouldConsiderCapacity()
    {
        var elevators = new List<Elevator>
            {
                CreateElevator(1, 5, ElevatorStatus.Moving, Direction.Up, DefaultElevatorCapacity),
                CreateElevator(2, 6, ElevatorStatus.Moving, Direction.Up, 2)
            };

        var selectedElevator = _strategy.SelectElevator(elevators, 10, Direction.Up);

        Assert.Equal(2, selectedElevator.Id);
    }

    [Fact]
    public void SelectElevator_ShouldThrowException_WhenNoElevatorsAvailable()
    {
        var elevators = new List<Elevator>
        {
            CreateElevator(1, 5, ElevatorStatus.OutOfService, Direction.Stationary, 0)
        };

        Assert.Throws<InvalidOperationException>(() => _strategy.SelectElevator(elevators, 3, Direction.Up));
    }

    [Fact]
    public void GetNextDestination_ShouldReturnPassengerDestination_WhenPassengersInElevator()
    {
        var elevator = CreateElevator(1, 5, ElevatorStatus.Moving, Direction.Up, 2);
        elevator.Passengers.Add(new Passenger(1, 8, 70));
        elevator.Passengers.Add(new Passenger(2, 10, 70));

        var building = CreateMockBuilding(10);

        var nextDestination = _strategy.GetNextDestination(elevator, building);

        Assert.Equal(5, nextDestination);  // The elevator returns to its current floor if there are no requests
    }

    [Fact]
    public void GetNextDestination_ShouldReturnNearestFloorWithRequest_WhenNoPassengersInElevator()
    {
        var elevator = CreateElevator(1, 5, ElevatorStatus.Moving, Direction.Up, 0);
        var building = CreateMockBuilding(10);
        SetupMockFloorWithRequest(building.Floors[7], Direction.Up);

        var nextDestination = _strategy.GetNextDestination(elevator, building);

        Assert.Equal(8, nextDestination);
    }

    [Fact]
    public void GetNextDestination_ShouldStayAtCurrentFloor_WhenNoRequestsInCurrentDirection()
    {
        var elevator = CreateElevator(1, 5, ElevatorStatus.Moving, Direction.Up, 0);
        var building = CreateMockBuilding(10);
        SetupMockFloorWithRequest(building.Floors[2], Direction.Down);

        var nextDestination = _strategy.GetNextDestination(elevator, building);

        Assert.Equal(5, nextDestination);  // Should return the current floor
        Assert.Equal(Direction.Up, elevator.Direction);  // Direction should remain unchanged
    }

    private Elevator CreateElevator(int id, int currentFloor, ElevatorStatus status, Direction direction, int passengerCount)
    {
        var elevator = new Elevator(id, DefaultElevatorCapacity, DefaultElevatorWeightLimit)
        {
            CurrentFloor = currentFloor,
            Status = status,
            Direction = direction
        };

        for (int i = 0; i < passengerCount; i++)
        {
            elevator.Passengers.Add(new Passenger(i, currentFloor + 1, 70));
        }

        return elevator;
    }

    private Building CreateMockBuilding(int floorCount)
    {
        var building = new Building(floorCount, 1, DefaultElevatorCapacity, DefaultElevatorWeightLimit);
        for (int i = 0; i < floorCount; i++)
        {
            building.Floors.Add(new Floor(i + 1));
        }
        return building;
    }

    private void SetupMockFloorWithRequest(Floor floor, Direction direction)
    {
        floor.ElevatorCallButtons[direction] = true;
    }
}
