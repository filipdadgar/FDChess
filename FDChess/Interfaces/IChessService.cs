using FDChess.Model;

namespace FDChess.Interfaces
{
    public interface IChessService
    {
        string MakeMove(Board board, Position newPosition);
        string GetGameState();
        void SetGameState(string gameState);
        Game CreateGame(string name, string state, Board board);
        Piece? AddPieceToBoard(Board board, string name, Position position, string color);
        List<Piece?> GetPieces(Board board);
    }
}
