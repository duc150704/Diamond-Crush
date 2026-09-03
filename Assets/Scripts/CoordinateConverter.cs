using UnityEngine;

public class CoordinateConverter : ICoordinateConverter
{
    public Vector3 GridToWorld<T>(GridPosition position, CustomizedGrid<T> grid)
    {
        throw new System.NotImplementedException();
    }

    public GridPosition WorldToGrid<T>(Vector3 position, CustomizedGrid<T> grid)
    {
        throw new System.NotImplementedException();
    }
}
