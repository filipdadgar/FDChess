namespace FDChess.Model;

public struct Position
{
    public int Row { get; set; }
    public int Column { get; set; }

    public Position(int row, int column)
    {
        Row = row;
        Column = column;
    }

    // Parameterless constructor for deserialization
    public Position() : this(0, 0) { }

    // Conversion constructor for positions like "A1" to (0, 0)
    public Position(string pos)
    {
        // Implement conversion logic
        Row = pos[1] - '1';
        Column = pos[0] - 'A';
    }

    // ToString method to convert back to chess notation (e.g., "A1")
    public override string ToString()
    {
        return $"{(char)('A' + Column)}{Row + 1}";
    }

    // Equality comparison to simplify checking positions
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