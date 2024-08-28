using ElevatorChallenge.Application.Services;
using ElevatorChallenge.Core.Entities;
using ElevatorChallenge.Core.Interfaces;
using ElevatorChallenge.Infrastructure.UserInterface;
using Microsoft.Extensions.DependencyInjection;

namespace ElevatorChallenge.ConsoleApp;

public class Startup
{
    public IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<Building>(x => new Building(10, 3, 8, 2000));
        services.AddSingleton<IElevatorService, ElevatorService>();
        services.AddSingleton<IConsoleUserInterface, ConsoleUserInterface>();
        services.AddSingleton<SimulationEngine>();

        return services.BuildServiceProvider();
    }
}
