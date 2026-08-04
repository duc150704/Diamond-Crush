using System.Collections.Generic;
public class MatchResult 
{
    public List<GridPosition> MatchedGridPosition;
    public int Length => MatchedGridPosition.Count;

    public MatchResult()
    {
        MatchedGridPosition = new List<GridPosition>(7);
    }

    public void Add(GridPosition gridPosition)
    {
        MatchedGridPosition.Add(gridPosition);
    }

}


