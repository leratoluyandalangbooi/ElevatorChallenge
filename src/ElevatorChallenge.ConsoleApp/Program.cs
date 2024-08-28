using ElevatorChallenge.Application.Services;
using ElevatorChallenge.ConsoleApp;
using Microsoft.Extensions.DependencyInjection;

namespace ElevatorChalleng.ConsoleApp;

class Program
{
    static async Task Main(string[] args)
    {
        var startUp = new Startup();
        var serviceProvider = startUp.ConfigureServices();

        var simulationEngine = serviceProvider.GetRequiredService<SimulationEngine>();
        await simulationEngine.RunSimulation();
    }
}