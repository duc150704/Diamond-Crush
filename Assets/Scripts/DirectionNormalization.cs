using UnityEngine;

public enum DragDirection
{
    Zero, Left, Right, Up, Down
}

public struct GridNeighbor
{
    public static readonly GridPosition Zero = new GridPosition(0, 0);  
    public static readonly GridPosition Up = new GridPosition(1, 0);  
    public static readonly GridPosition Left= new GridPosition(0, 0);  
    public static readonly GridPosition Down = new GridPosition(0, 0);  
    public static readonly GridPosition Right = new GridPosition(0, 0);  
}

public static class DirectionNormalization
{
    public static DragDirection Normalize(Vector3 dir)
    {
        dir.Normalize();
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        DragDirection res = DragDirection.Zero;

        if(-30 <= angle && angle <= 30)
        {
            res = DragDirection.Right;
        } 
        else if (60 <= angle && angle <= 120)
        {
            res = DragDirection.Up;
        }
        else if (-120 <= angle && angle <= -60)
        {
            res = DragDirection.Down;
        }
        else if ((150 <= angle && angle <= 180) || (-180 <= angle && angle <= -150))
        {
            res = DragDirection.Left;
        }

        return res;
    }
}
