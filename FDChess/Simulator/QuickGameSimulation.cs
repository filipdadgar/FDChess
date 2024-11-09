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
            new MoveRequest { CurrentPosition = new Position(1, 4), NewPosition = new Position(3, 4) }, // White pawn to e4
            new MoveRequest { CurrentPosition = new Position(6, 4), NewPosition = new Position(4, 4) }, // Black pawn to e5
            new MoveRequest { CurrentPosition = new Position(0, 3), NewPosition = new Position(4, 7) }, // White queen to h5
            new MoveRequest { CurrentPosition = new Position(7, 1), NewPosition = new Position(5, 2) }, // Black knight to c6
            new MoveRequest { CurrentPosition = new Position(4, 7), NewPosition = new Position(6, 7) }  // White queen to h7 (checkmate)
        };

        foreach (var move in moves)
        {
            var response = _chessService.MakeMove(move);
            Console.WriteLine(response);
        }

        var finalState = _chessService.GetGameState();
        Console.WriteLine(finalState);
    }
}