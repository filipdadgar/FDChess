namespace FDChess.Model
{
    /// <summary>
    /// Chess Board Class
    /// </summary>
    public class Board
    {
        /// <summary>
        /// Chess Board Constructor
        /// </summary>
        public Board()
        {
        }

        public Board(int id) {
            Id = id;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public override string ToString() 
        {
            return $"Board {Id}: {Name} - {Description}";
        }

    }
}
