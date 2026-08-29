using UnityEngine;

public enum DragDirection
{
    Zero, Left, Right, Up, Down
}

public static class GridOffset
{
    public static readonly GridPosition Zero = new GridPosition(0, 0);
    public static readonly GridPosition Up = new GridPosition(0, 1);
    public static readonly GridPosition Left = new GridPosition(-1, 0);
    public static readonly GridPosition Down = new GridPosition(0, -1);
    public static readonly GridPosition Right = new GridPosition(1, 0);

    public static GridPosition GetFromVector(Vector3 vector)
    {
        vector.Normalize();
        float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

        GridPosition result = Zero;

        if (-30 <= angle && angle <= 30)
        {
            result = Right;
        }
        else if (60 <= angle && angle <= 120)
        {
            result = Up;
        }
        else if (-120 <= angle && angle <= -60)
        {
            result = Down;
        }
        else if ((150 <= angle && angle <= 180) || (-180 <= angle && angle <= -150))
        {
            result = Left;
        }

        return result;
    }
}
