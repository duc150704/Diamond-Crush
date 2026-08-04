using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeGrid<T> where T : IOnGrid
{
    public int Width { get; }
    public int Height { get; }
    
    public float CellSize { get; }

    public event Action<GridPosition> OnGridValueChanged;

    private T[,] _values;
    private Vector3 _translationVector = Vector3.zero;
    
    public CustomizeGrid(int width, int height, float cellSize, Vector3 centerPostion)
    {
        Width = width;
        Height = height;
        CellSize = cellSize;
        _values = new T[width, height];

        Vector3 gridCenter = ((GridToWorld(new GridPosition(0, 0)) + GridToWorld(new GridPosition(width - 1, height - 1)))) / 2;
        _translationVector = centerPostion - gridCenter;

        Debug.Log($"{gridCenter} \n{_translationVector}");
    }

    public bool Swap(GridPosition p1, GridPosition p2)
    {
        if(!IsValid(p2) || !IsValid(p1))
            return false;

        T obj1 = GetValue(p1);
        T obj2 = GetValue(p2);

        return SetValue(p1, obj2) && SetValue(p2, obj1);
    }

    public List<T> GetAllValue()
    {
        List<T> list = new List<T>();

        for(int i = 0; i < Width; i++)
        {
            for(int j = 0; j < Height; j++)
            {
                list.Add(GetValue(new GridPosition(i,j)));
            }
        }

        return list;
    }

    public T GetValue(Vector3 worldPosition)
    {
        return GetValue(WorldToGrid(worldPosition));
    }

    public T GetValue(GridPosition gridPosition)
    {
        if(!IsValid(gridPosition))
            return default(T);

        return _values[gridPosition.x, gridPosition.y];
    }

    public bool SetValue(GridPosition gridPosition, T value)
    {
        if (!IsValid(gridPosition))
            return false;

        if(value != null)
            value.SetGridPosition(gridPosition);
        _values[gridPosition.x, gridPosition.y] = value;

        OnGridValueChanged?.Invoke(gridPosition);

        return true;
    }

    public Vector3 GridToWorld(GridPosition gridPosition)
    {
        float x = (gridPosition.x + 0.5f) * CellSize;
        float y = (gridPosition.y + 0.5f) * CellSize;

        return new Vector3(x, y) + _translationVector;
    }

    public GridPosition WorldToGrid(Vector3 worldPosition)
    {
        worldPosition -= _translationVector;

        int x = Mathf.FloorToInt(worldPosition.x / CellSize);
        int y = Mathf.FloorToInt(worldPosition.y / CellSize);

        return new GridPosition(x, y);
    }

    private bool IsValid(GridPosition gridPosition)
        => 0 <= gridPosition.x && gridPosition.x < Width && 0 <= gridPosition.y && gridPosition.y < Height;
}
