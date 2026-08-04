using System.Collections.Generic;

public static class RefillSystem 
{
    public static List<GridPosition> FindRefillPosition(CustomizeGrid<GridObject> grid)
    {
        var list = new List<GridPosition>();

        for(int i = 0;i < grid.Width; i++)
        {
            int down = grid.Height - 1;

            while(down >= 0)
            {
                GridObject objDown = grid.GetValue(new GridPosition(i, down));

                if (objDown != null)
                    break;

                list.Add(new GridPosition(i, down));
                down--;
            }
        }

        return list;
    }

    public static void Fill(CustomizeGrid<GridObject> grid, List<GridPosition> gridPositions, int from = 1, int to = 2)
    {
        foreach (var gridPos in gridPositions)
        {
            grid.SetValue(gridPos, new GridObject(NETUltilities.GetRandomInt(from, to)));  // to : Exclusive
        }
    }
}
