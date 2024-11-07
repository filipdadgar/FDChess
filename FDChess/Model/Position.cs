using System.Text.Json.Serialization;

namespace FDChess.Model
{
    public struct Position : IEquatable<Position>
    {
        public int Row { get; set; }
        public int Column { get; set; }

        [JsonConstructor]
        public Position(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public Position(string pos)
        {
            Row = pos[1] - '1';
            Column = pos[0] - 'A';
        }

        public override string ToString()
        {
            return $"{(char)('A' + Column)}{Row + 1}";
        }

        public bool Equals(Position other)
        {
            return Row == other.Row && Column == other.Column;
        }

        public override bool Equals(object obj)
        {
            if (obj is Position other)
            {
                return Row == other.Row && Column == other.Column;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Column);
        }
    }
}