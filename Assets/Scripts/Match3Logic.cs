using System.Collections.Generic;

public enum LineDirection
{
    Vertical,
    Horizontal
}

public static class Match3Logic
{
    public static void ShuffeGrid(CustomizedGrid<GridObject> grid , List<DiamondSO> data)
    {
        CustomizedGrid<GridObject> clone = grid.Clone();
        int time = 0;

        List<GridData<GridObject>> gridData = new List<GridData<GridObject>>();
        clone.SetData(gridData);
        int move = FindMoves(clone).Count;

        while (move <= 0 && time <= 20)
        {
            time++;
            gridData = GenerateGridData(clone.Columns, clone.Rows, data);
            clone.SetData(gridData);
            move = FindMoves(clone).Count;
        }

        grid.SetData(gridData);
    }

    public static List<GridData<GridObject>> GenerateGridData(int columns, int rows, List<DiamondSO> diamonds)
    {
        List<GridData<GridObject>> list = new List<GridData<GridObject>>();

        for (int c = 0; c < columns; c++)
        {
            for (int r = 0; r < rows; r++)
            {
                GridData<GridObject> data = new GridData<GridObject>()
                {
                    GridPosition = new GridPosition(c, r),
                    Value = new GridObject(NETUltilities.GetRandomInt(0, diamonds.Count))
                };
                
                data.Value.GridPosition = new GridPosition(c, r);
                list.Add(data);
            }
        }

        return list;
    }

    public static BestMove FindBestMove(CustomizedGrid<GridObject> grid)
    {
        BestMove bestMove = new BestMove();
        List<Move> moves = FindMoves(grid);

        foreach (Move move in moves)
        {
            if (bestMove.ResultLength < move.GetMatch().MatchedGridPositions.Count)
            {
                bestMove.From = move.From;
                bestMove.To = move.To;
                bestMove.Result.Clear();

                foreach (var item in move.GetMatch().MatchedGridPositions)
                {
                    if (item == move.From)
                        bestMove.Result.Add(move.To);
                    else if (item == move.To)
                        bestMove.Result.Add(move.From);
                    else
                        bestMove.Result.Add(item);
                }
            }
        }

        return bestMove;
    }

    private static void MergeResult(List<MatchResult> mrl)
    {
        UnionFind unionFind = new UnionFind(mrl.Count);

        Dictionary<GridPosition, int> positionToRoot = new(); 
        for (int currentRoot = 0; currentRoot < mrl.Count; currentRoot++) // kiem tra Position da xuat hien chua
        {
            foreach (var pos in mrl[currentRoot].MatchedGridPositions)
            {
                if (!positionToRoot.TryGetValue(pos, out int oldRoot))
                   positionToRoot[pos] = currentRoot;
                else
                    unionFind.Union(currentRoot, oldRoot);
            }
        }

        Dictionary<int, List<int>> rootToListChildIndex = new(); //nhom cac matches co cung root lai qua index
        for (int i = 0; i < mrl.Count; i++)
        {
            int root = unionFind.Find(i);

            if (!rootToListChildIndex.ContainsKey(root))
                rootToListChildIndex[root] = new List<int>();

            rootToListChildIndex[root].Add(i);
        }

        bool[] isUsed = new bool[mrl.Count];
        foreach (var intList in rootToListChildIndex.Values) // gop matches;
        {
            MatchResult mr = mrl[intList[0]]; // lay ra matches dau tien trong moi nhom de gop;
            for (int i = 1; i < intList.Count; i++)
            {
                mr.MatchedGridPositions.UnionWith(mrl[intList[i]].MatchedGridPositions);
            }
            isUsed[intList[0]] = true;
        }

        for (int i = mrl.Count - 1; i >= 0; i--) // bo matches da gop vao mathes khac
        {
            if (!isUsed[i])
                mrl.RemoveAt(i);
        }
    }

    public static Move FindMove(CustomizedGrid<GridObject> grid, GridPosition from, GridPosition to)
    {
        Move move = new();
        grid.Swap(from, to);

        move.From = from;
        move.To = to;

        move.Results.AddRange(CheckLine(grid, from.Column, LineDirection.Vertical));
        move.Results.AddRange(CheckLine(grid, from.Row, LineDirection.Horizontal));

        move.Results.AddRange(CheckLine(grid, to.Column, LineDirection.Vertical));
        move.Results.AddRange(CheckLine(grid, to.Row, LineDirection.Horizontal));

        MergeResult(move.Results);

        grid.Swap(from, to);

        return move;
    }

    public static List<Move> FindMoves(CustomizedGrid<GridObject> grid)
    {
        List<Move> possibleMoves = new List<Move>();

        for (int column = 0; column < grid.Columns; column++)
        {
            for (int row = 0; row < grid.Rows; row++)
            {
                GridPosition currentPosition = new GridPosition(column, row);
                GridPosition targetPosition = new GridPosition();

                targetPosition = currentPosition.GetRight();
                Move moveA = FindMove(grid, currentPosition, targetPosition);
                if(moveA.MatchCount > 0)
                    possibleMoves.Add(moveA);

                targetPosition = currentPosition.GetUp();
                Move moveB = FindMove(grid, currentPosition, targetPosition);
                if (moveB.MatchCount > 0)
                    possibleMoves.Add(moveB);
            }
        }

        return possibleMoves;
    }

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

            GridObject startObj = null;
            GridObject currentObj = null;

            switch (lineDirection)
            {
                case LineDirection.Vertical:
                    grid.TryGet(new GridPosition(line, start), out startObj);
                    grid.TryGet(new GridPosition(line, current), out currentObj);

                break;
                case LineDirection.Horizontal:
                    grid.TryGet(new GridPosition(start, line), out startObj);
                    grid.TryGet(new GridPosition(current, line), out currentObj);

                break;
            }

            if (startObj == null || currentObj == null)
            {
                if (current - start >= 3)
                {
                    mrl.Add(CreateMatchResult(start, current, line, lineDirection));
                }
                start = current;
                current++;
                continue;
            }

            if (startObj.ItemType == currentObj.ItemType)
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
                    ObjectType = tmp.ItemType
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
