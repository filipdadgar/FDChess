namespace FDChess.Model
{
    /// <summary>
    /// Chess Piece Class
    /// </summary>
    public class Piece
    {
        /// <summary>
        /// Chess Piece Constructor
        /// </summary>
        public Piece()
        {
        }

        public Piece(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public override string ToString()
        {
            return $"Piece {Id}: {Name} - {Description}";
        }

    }
}
