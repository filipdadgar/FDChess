using System.Collections.Generic;

public class PositionService
{
    private List<Position> positions = new List<Position>();

    public void AddPosition(Position position)
    {
        positions.Add(position);
    }

    public void ClearPositions()
    {
        positions.Clear();
    }
}