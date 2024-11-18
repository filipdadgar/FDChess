using System.Text.Json;
using FDChess.Controllers;
using FDChess.Model;

namespace FDChess.Interfaces
{
    public interface IChessService
    {
        string MakeMove(MoveRequest moveRequest);
        string GetGameState();
        void SetGameState(string gameState);
        Game CreateGame(string name, string state, Board board);
        Piece? AddPieceToBoard(Board board, string name, Position position, string color);
        List<Piece> InitializeDefaultPieces();
        void ResetGame();
        List<Position> GetPossibleMoves(int pieceId);
        public List<Piece> GetRemovedPieces();
        public List<Piece> GetAvailablePieces();
        string PromotePawn(Position position, string newPieceType);
        Task<string> DescribeBoardAsync();
    }
}
