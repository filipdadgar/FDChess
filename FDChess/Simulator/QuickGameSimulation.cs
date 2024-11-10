using FDChess.Controllers;
using FDChess.Model;
using FDChess.Services;

public class QuickGameSimulation
{
    private readonly ChessService _chessService;

    public QuickGameSimulation()
    {
        _chessService = new ChessService();
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
