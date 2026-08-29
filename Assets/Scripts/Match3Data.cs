using System.Collections.Generic;

public struct SpawnData
{
    public GridObject GridObject;
    public GridPosition Position;
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

public struct PossibleMove
{
    public GridPosition from;
    public GridPosition to;
}
