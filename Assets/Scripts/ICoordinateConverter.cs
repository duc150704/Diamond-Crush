using UnityEngine;

public interface ICoordinateConverter
{
    public Vector3 GridToWorld<T>(GridPosition position, CustomizedGrid<T> grid);
    public GridPosition WorldToGrid<T>(Vector3 position, CustomizedGrid<T> grid);
}
