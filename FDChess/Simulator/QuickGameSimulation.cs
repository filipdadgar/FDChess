using FDChess.Controllers;
using FDChess.Model;
using FDChess.Services;
using Microsoft.FeatureManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class QuickGameSimulation
{
    private readonly ChessService _chessService;

    public QuickGameSimulation()
    {
        // Create a service collection
        var serviceCollection = new ServiceCollection();

        // Add configuration
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        serviceCollection.AddSingleton<IConfiguration>(configuration);

        // Add feature management
        serviceCollection.AddFeatureManagement(configuration.GetSection("FeatureManagement"));

        // Build the service provider
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Get the feature manager
        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        // Optionally create an AIService instance if needed
        AIService? aiService = null;
        if (featureManager.IsEnabledAsync("AIService").Result)
        {
            aiService = new AIService();
        }

        // Create the ChessService instance
        _chessService = new ChessService(featureManager, aiService);
    }

    public void Run()
    {
        var moves = new List<MoveRequest>
        {
            // Scholar's Mate sequence
            new MoveRequest { CurrentPosition = new Position(1, 4), NewPosition = new Position(3, 4) }, // e4
            new MoveRequest { CurrentPosition = new Position(6, 4), NewPosition = new Position(4, 4) }, // e5
            new MoveRequest { CurrentPosition = new Position(0, 5), NewPosition = new Position(3, 2) }, // Bc4
            new MoveRequest { CurrentPosition = new Position(6, 5), NewPosition = new Position(5, 5) }, // f6
            new MoveRequest { CurrentPosition = new Position(0, 3), NewPosition = new Position(4, 7) }, // Qh5
            new MoveRequest { CurrentPosition = new Position(7, 6), NewPosition = new Position(5, 5) }, // Nf6
            new MoveRequest { CurrentPosition = new Position(4, 7), NewPosition = new Position(6, 7) }  // Qxf7#
        };

        foreach (var move in moves)
        {
            var response = _chessService.MakeMove(move);
            Console.WriteLine($"Move result: {response}");
            Console.WriteLine($"Board state after move:");
            Console.WriteLine(_chessService.GetGameState());
        }
    }
}

