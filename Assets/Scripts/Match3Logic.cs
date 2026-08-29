using System.Collections.Generic;

public enum LineDirection
{
    Vertical,
    Horizontal
}

public static class Match3Logic
{
    //public static List<PossibleMove> FindPossibleMoves(CustomizedGrid<GridObject> grid)
    //{

    //}

    public static List<MatchResult> CheckLine(CustomizedGrid<GridObject> grid, int line, LineDirection lineDirection)
    {
        int current = 0;
        int start = 0;

        int max = 0;

        if (lineDirection == LineDirection.Vertical)
            max = grid.Rows;
        else if (lineDirection == LineDirection.Horizontal)
            max = grid.Columns;

        List<MatchResult> mrl = new List<MatchResult>();

        for (int i = 0; i < max; i++)
        {
            if (start == current)
            {
                current++;
                continue;
            }

            int typeOfCurrent = 0;
            int typeOfStart = 0;

            switch (lineDirection)
            {
                case LineDirection.Vertical:
                    typeOfCurrent = grid.Get(new GridPosition(line, start)).ItemType;
                    typeOfStart = grid.Get(new GridPosition(line, current)).ItemType;

                break;
                case LineDirection.Horizontal:
                    typeOfCurrent = grid.Get(new GridPosition(start, line)).ItemType;
                    typeOfStart = grid.Get(new GridPosition(current, line)).ItemType;

                break;
            }

            if (typeOfCurrent == typeOfStart)
            {
                current++;
                continue;
            }

            if (current - start >= 3)
            {
                mrl.Add(CreateMatchResult(start, current, line, lineDirection));
            }

            start = current;
            current++;
        }

        if (current - start >= 3)
        {
            mrl.Add(CreateMatchResult(start, current, line, lineDirection));
        }

        return mrl;
    }

    private static MatchResult CreateMatchResult(int start, int end, int line, LineDirection lineDirection)
    {
        MatchResult mr = new MatchResult();

        for (int i = start; i < end; i++) 
        {
            if (lineDirection == LineDirection.Vertical) 
                mr.MatchedGridPositions.Add(new GridPosition(line, i));
            else if (lineDirection == LineDirection.Horizontal)
                mr.MatchedGridPositions.Add(new GridPosition(i, line));
        }

        ShapeDetector.DetectMinPivot(mr.MatchedGridPositions, out int xMin, out int yMin);
        mr.XMin = xMin;
        mr.YMin = yMin;

        mr.Shape = ShapeDetector.DetectShape(mr.MatchedGridPositions, mr.XMin, mr.YMin);

        return mr;
    }

    public static RefillResult Fill(CustomizedGrid<GridObject> grid, List<RefillablePositionData> gridPositions, int from, int to)
    {
        List<SpawnData> spawnData = new List<SpawnData>();
        List<FallResult> fallData = new List<FallResult>();
        foreach (var gridPos in gridPositions)
        {
            GridObject tmp = new GridObject(NETUltilities.GetRandomInt(from, to));
            grid.Set(gridPos.GridPosition, tmp);
            tmp.GridPosition = gridPos.GridPosition;

            spawnData.Add(
                new SpawnData()
                {
                    GridObject = tmp,
                    Position = gridPos.PositionOffset,
                }
            );

            fallData.Add(
                new FallResult()
                {
                    GridObject = tmp,
                    TargetPosition = gridPos.GridPosition,
                }
            );
        }

        return new RefillResult() 
        {
            SpawnData = spawnData,
            FallData = fallData
        };
    }

    public static List<RefillablePositionData> FindRefillablePosition_2(CustomizedGrid<GridObject> grid)
    {
        List<RefillablePositionData> data = new();

        for (int column = 0; column < grid.Columns; column++)
        {
            int row = 0;
            GridPosition minPos = new GridPosition(0, 0);
            while(row < grid.Rows)
            {
                GridObject curObj = grid.Get(new GridPosition(column, row));

                if (curObj != null)
                {
                    row++;
                    continue;
                }

                if (minPos == GridPosition.Zero)
                    minPos = new GridPosition(0, row);

                data.Add(
                    new RefillablePositionData()
                    {
                        GridPosition = new GridPosition(column, row),
                        PositionOffset = new GridPosition(column, grid.Rows + (row - minPos.Row))
                    }
                );

                row++;
            }
        }

        return data;
    }

    public static List<FallResult> ApplyGravity(CustomizedGrid<GridObject> grid)
    {
        List<FallResult> fallData = new List<FallResult>();

        for (int i = 0; i < grid.Columns; i++)
        {
            int up = 0;
            int down = 0;

            while (up < grid.Rows)
            {
                GridObject objDown = grid.Get(new GridPosition(i, down));
                GridObject objUp = grid.Get(new GridPosition(i, up));

                if (objUp == null)
                {
                    up++;
                    continue;
                }

                if (down != up)
                {

                    grid.Set(new GridPosition(i, down), objUp);
                    grid.Set(new GridPosition(i, up), null);

                    objUp.GridPosition = new GridPosition(i, down);

                    fallData.Add(
                        new FallResult() 
                        { 
                            GridObject = objUp, 
                            TargetPosition = objUp.GridPosition
                        }
                    );
                }

                up++;
                down++;
            }
        }

        return fallData;
    }

    public static void RemoveMatches(CustomizedGrid<GridObject> grid, MatchFinalResult matchFinalResult)
    {
        foreach (var pos in matchFinalResult.MatchedGridPositions)
        {
            grid.Set(pos, null);
        }
    }

    public static SwapResult Swap(CustomizedGrid<GridObject> grid, GridPosition firstPos, GridPosition secondPos)
    {
        SwapResult result = new SwapResult();

        if (!grid.TryGet(firstPos, out GridObject firstObj) || firstObj == null)
        {
            result.IsSuccess = false;
            return result;
        }

        if (!grid.TryGet(secondPos, out GridObject secondObj) || secondObj == null)
        {
            result.IsSuccess = false;
            return result;
        }

        grid.Set(firstPos, secondObj);
        grid.Set(secondPos, firstObj);

        firstObj.GridPosition = secondPos;
        secondObj.GridPosition = firstPos;

        result.FirstObject = firstObj;
        result.SecondObject = secondObj;

        result.IsSuccess = true;

        return result;
    }

    public static MatchFinalResult FindMatches(CustomizedGrid<GridObject> grid)
    {
        MatchFinalResult mfr = new MatchFinalResult();

        Dictionary<GridPosition, GridPosition> gridPositionToRoot = new();
        Dictionary<GridPosition, MatchResult> rootToResult = new();

        for (int column = 0; column < grid.Columns; column++)
        {
            int down = 0;
            int up = 0;

            while (up < grid.Rows)
            {
                if(up == down || grid.Get(new GridPosition(column, down)).ItemType == grid.Get(new GridPosition(column, up)).ItemType)
                {
                    up++;
                    continue;
                }

                if(up - down >= 3)
                {
                    MatchResult mr = new();
                    GridPosition root = new GridPosition(column, down);
                    for (int k = down; k < up; k++)
                    {
                        GridPosition curPos = new GridPosition(column, k);
                        gridPositionToRoot.Add(curPos, root);
                        mr.MatchedGridPositions.Add(curPos);
                    }
                    rootToResult.Add(root, mr);
                }

                down = up;
                up++;
            }
            if (up - down >= 3)
            {
                MatchResult mr = new();
                GridPosition root = new GridPosition(column, down);
                for (int k = down; k < up; k++)
                {
                    GridPosition curPos = new GridPosition(column, k);
                    gridPositionToRoot.Add(curPos, root);
                    mr.MatchedGridPositions.Add(curPos);
                }
                rootToResult.Add(root, mr);
            }
        }


        for (int row = 0; row < grid.Rows; row++)
        {
            int left = 0;
            int right = 0;

            while (right < grid.Columns)
            {
                if (left == right|| grid.Get(new GridPosition(left , row)).ItemType == grid.Get(new GridPosition(right, row)).ItemType)
                {
                    right++;
                    continue;
                }

                if (right - left >= 3)
                {
                    MatchResult mr = new();
                    GridPosition newRoot = new GridPosition(left, row);

                    for(int k = left; k < right; k++)
                    {
                        GridPosition curPos = new GridPosition(k, row);
                        if(gridPositionToRoot.TryGetValue(curPos, out GridPosition root)
                            && rootToResult.TryGetValue(root, out MatchResult tmpMr))
                        {
                            mr.MatchedGridPositions.UnionWith(tmpMr.MatchedGridPositions);
                            foreach (var item in tmpMr.MatchedGridPositions)
                            {
                                gridPositionToRoot[item] = newRoot;
                            }
                            rootToResult.Remove(root);
                        }
                        else
                        {
                            gridPositionToRoot.TryAdd(curPos, newRoot);
                            mr.MatchedGridPositions.Add(curPos);
                        }
                    }
                    rootToResult.Add(newRoot, mr);
                }

                left = right;
                right++;
            }

            if (right - left >= 3)
            {
                MatchResult mr = new();
                GridPosition newRoot = new GridPosition(left, row);

                for (int k = left; k < right; k++)
                {
                    GridPosition curPos = new GridPosition(k, row);
                    if (gridPositionToRoot.TryGetValue(curPos, out GridPosition root)
                        && rootToResult.TryGetValue(root, out MatchResult tmpMr))
                    {
                        mr.MatchedGridPositions.UnionWith(tmpMr.MatchedGridPositions);
                        foreach (var item in tmpMr.MatchedGridPositions)
                        {
                            gridPositionToRoot[item] = newRoot;
                        }
                        rootToResult.Remove(root);
                    }
                    else
                    {
                        gridPositionToRoot.TryAdd(curPos, newRoot);
                        mr.MatchedGridPositions.Add(curPos);
                    }
                }
                rootToResult.Add(newRoot, mr);
            }
        }


        foreach (var item in rootToResult.Values)
        { 
            foreach (var gridPosition in item.MatchedGridPositions)
            {
                mfr.MatchedGridPositions.Add(gridPosition);
                mfr.MatchedObjs.Add(grid.Get(gridPosition));
            }
            ShapeDetector.DetectMinPivot(item.MatchedGridPositions, out int x, out int y);
            item.XMin = x;
            item.YMin = y;
            item.Shape = ShapeDetector.DetectShape(item.MatchedGridPositions, item.XMin, item.YMin);
            mfr.MatchResults.Add(item);
        }

        return mfr;
    }


}
