using System.Text.Json.Serialization;

namespace FDChess.Model
{
    /// <summary>
    /// Chess Game Class
    /// </summary>
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public Board Board { get; set; }

        [JsonConstructor]
        public Game()
        {
            Board = new Board();
        }

        public Game(int id, string name, string state, Board board)
        {
            Id = id;
            Name = name;
            State = state;
            Board = board;
        }

        public override string ToString()
        {
            return $"{Name} - {State}";
        }
    }

}
