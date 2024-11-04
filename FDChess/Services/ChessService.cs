using FDChess.Model;

namespace FDChess.Services
{
    /// <summary>
    /// Chess Service Class
    /// </summary>
    public class ChessService
    {

        // Variables
        private string _gameState;
        private string _gameName;
        private string _gameVersion;


        public ChessService(string gameVersion, string gameName, string gameState)
        {
            _gameVersion = gameVersion;
            _gameName = gameName;
            _gameState = gameState;
        }

        public string MakeMove(Board request)
        {
            return "Move Made";
        }

        public string GetGameState()
        {
            return "Game State";
        }
        
    }
}
