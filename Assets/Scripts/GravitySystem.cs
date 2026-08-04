public static class GravitySystem 
{
    public static void Apply(CustomizeGrid<GridObject> grid)
    {
        for (int i = 0; i < grid.Width; i++)
        {
            int up = 0;
            int down = 0;

            while (up < grid.Height)
            {
                GridObject objDown = grid.GetValue(new GridPosition(i, down));
                GridObject objUp = grid.GetValue(new GridPosition(i, up));

                if (objUp == null)
                {
                    up++;
                    continue;
                }

                if (down != up)
                {
                    grid.SetValue(new GridPosition(i, down), objUp);
                    grid.SetValue(new GridPosition(i, up), objDown);
                }

                up++;
                down++;
            }
        }
    }
}
