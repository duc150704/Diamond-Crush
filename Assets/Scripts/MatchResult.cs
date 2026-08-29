using System.Collections.Generic;

public enum Shape
{
    None,
    Line3, Line4, Line5,
    Cross, 
    TShape,
    SquareConner,
    ChuaTinhDen
}

public class MatchResult 
{
    public HashSet<GridPosition> MatchedGridPositions { get; set; } = new();
    public int XMin { get; set; } = int.MaxValue;
    public int YMin { get; set; } = int.MaxValue;
    public Shape Shape { get; set; } = Shape.None;
}

public class MatchFinalResult
{
    public List<GridObject> MatchedObjs { get; set; } = new();
    public List<GridPosition> MatchedGridPositions { get; set; } = new();
    public List<MatchResult> MatchResults { get; set; } = new();
    public bool HasMatches => MatchResults.Count > 0;
}


