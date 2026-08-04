using System.Collections.Generic;

public static class MatchChecker
{
    public static List<MatchResult> FindMatches(CustomizeGrid<GridObject> grid)
    {
        List<MatchResult> res = new List<MatchResult>();

        res.AddRange(VerticalCheck(grid));
        res.AddRange(HorizontalCheck(grid));

        return res;
    }

    public static List<MatchResult> VerticalCheck(CustomizeGrid<GridObject> grid)
    {
        List<MatchResult> res = new List<MatchResult>();

        for (int i = 0; i < grid.Width; i++)
        {
            MatchResult tmpResult = new MatchResult();
            int down = 0;
            int up = 0;
            
            while (up < grid.Height)
            {
                GridObject objDown = grid.GetValue(new GridPosition(i, down));
                GridObject objUp = grid.GetValue(new GridPosition(i, up));

                if(objDown == null || objUp == null)
                {
                    if (tmpResult.Length >= 3)
                    {
                        res.Add(tmpResult);
                    }
                    tmpResult = new MatchResult();
                    down++;
                    up++;
                    continue;
                }

                if (down == up)
                {
                    tmpResult.Add(objDown.GridPosition);
                    up++;
                    continue;
                }

                if (objDown.ItemID == objUp.ItemID)
                {
                    tmpResult.Add(objUp.GridPosition);
                    up++;
                    continue;
                }

                if (tmpResult.Length >= 3)
                {
                    res.Add(tmpResult);
                }

                down = up;
                tmpResult = new MatchResult();
            }

            if (tmpResult.Length >= 3)
            {
                res.Add(tmpResult);
            }

        }

        return res;
    }    
    
    public static List<MatchResult> HorizontalCheck(CustomizeGrid<GridObject> grid)
    {
        List<MatchResult> res = new List<MatchResult>();

        for (int i = 0; i < grid.Height; i++)
        {
            MatchResult tmpResult = new MatchResult();
            int down = 0;
            int up = 0;
            
            while (up < grid.Width)
            {
                GridObject objDown = grid.GetValue(new GridPosition(down, i));
                GridObject objUp = grid.GetValue(new GridPosition(up, i));

                if(objDown == null || objUp == null)
                {
                    if (tmpResult.Length >= 3)
                    {
                        res.Add(tmpResult);
                    }
                    tmpResult = new MatchResult();
                    down++;
                    up++;
                    continue;
                }

                if (down == up)
                {
                    tmpResult.Add(objDown.GridPosition);
                    up++;
                    continue;
                }

                if (objDown.ItemID == objUp.ItemID)
                {
                    tmpResult.Add(objUp.GridPosition);
                    up++;
                    continue;
                }

                if (tmpResult.Length >= 3)
                {
                    res.Add(tmpResult);
                }

                down = up;
                tmpResult = new MatchResult();
            }

            if (tmpResult.Length >= 3)
            {
                res.Add(tmpResult);
            }

        }

        return res;
    }
}
