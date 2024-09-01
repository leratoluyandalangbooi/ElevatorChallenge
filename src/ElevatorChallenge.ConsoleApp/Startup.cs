using ElevatorChallenge.Application.Services;
using ElevatorChallenge.Core.Entities;
using ElevatorChallenge.Core.Interfaces;
using ElevatorChallenge.Application.Strategies;
using ElevatorChallenge.Infrastructure.UserInterface;
using Microsoft.Extensions.DependencyInjection;
using ElevatorChallenge.Infrastructure.Logging;
using ElevatorChallenge.Infrastructure.Wrappers;
using Microsoft.Extensions.Logging;

namespace ElevatorChallenge.ConsoleApp;

public class Startup
{
    public IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<Building>(x => new Building(10, 3, 8, 1000)); // 10 floors, 3 elevators, capacity 8, weight limit 1000
        services.AddSingleton<INearestAvailableElevatorStrategy, NearestAvailableElevatorStrategy>();
        services.AddSingleton<IElevatorService, ElevatorService>();
        services.AddSingleton<IConsoleUserInterface, ConsoleUserInterface>();
        services.AddSingleton<IConsoleWrapper, ConsoleWrapper>();
        services.AddSingleton<SimulationEngine>();

        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddFile("elevator-logs.txt");
        });

        services.AddSingleton<IElevatorLogger, ElevatorLogger>();

        Console.WriteLine("Elevator Simulation Started. Type 'quit' to exit.");
        return services.BuildServiceProvider();
    }
}
