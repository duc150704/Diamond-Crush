using System.Collections.Generic;

public struct SpawnData
{
    public GridObject GridObject;
    public GridPosition Position;
    public int ObjectType;
}

public struct SwapResult
{
    public bool IsSuccess;

    public GridObject FirstObject;
    public GridObject SecondObject;
}

public struct FallResult
{
    public GridObject GridObject;
    public GridPosition TargetPosition;
}

public struct RefillResult
{
    public List<SpawnData> SpawnData;
    public List<FallResult> FallData;
}

public struct RefillablePositionData
{
    public GridPosition GridPosition;
    public GridPosition PositionOffset;
}

public class Move
{
    public GridPosition From;
    public GridPosition To;
    public int MatchCount => Results.Count;
    public List<MatchResult> Results { get; set; } = new();

    public MatchResult GetMatch()
    {
        MatchResult matchResult = Results[0];
        for (int i = 1; i < Results.Count; i++)
        {
            if (Results[i].MatchedGridPositions.Count > matchResult.MatchedGridPositions.Count)
                matchResult = Results[i];
        }

        return matchResult;
    }
}

public class BestMove
{
    public GridPosition From;
    public GridPosition To;
    public List<GridPosition> Result  { get; set; } = new();
    public List<GridObject> Objects { get; set; } = new();
    public int ResultLength => Result.Count;
}

public class GridData<T>
{
    public GridPosition GridPosition { get; set; }
    public T Value { get; set; }
}
