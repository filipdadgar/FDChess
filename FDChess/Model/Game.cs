namespace FDChess.Model
{
    /// <summary>
    /// Chess Game Class
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Chess Board
        /// </summary>
        public Board Board { get; set; }

        /// <summary>
        /// Chess Pieces
        /// </summary>
        public Piece[] Pieces { get; set; }

        /// <summary>
        /// Chess Game Constructor
        /// </summary>
        public Game()
        {
            Board = new Board();
            Pieces = new Piece[32];
        }


    }
}
