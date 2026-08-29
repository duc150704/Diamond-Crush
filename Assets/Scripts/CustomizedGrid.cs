using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CustomizedGrid<T>
{
    public int Columns { get; }
    public int Rows { get; }
    public float CellSize { get; }

    public event Action<GridPosition> OnGridValueChanged;

    private T[,] _values;
    private Vector3 _translationVector = Vector3.zero;
    
    public CustomizedGrid(int columns, int rows, float cellSize, Vector3 centerPostion = default)
    {
        Columns = columns;
        Rows = rows;
        CellSize = cellSize;

        _values = new T[columns, rows];

        Vector3 gridCenter = ((GridToWorld(new GridPosition(0, 0)) + GridToWorld(new GridPosition(columns - 1, rows - 1)))) / 2;
        _translationVector = centerPostion - gridCenter;
    }

    public bool Swap(GridPosition p1, GridPosition p2)
    {
        if(!IsValid(p2) || !IsValid(p1))
            return false;

        T obj1 = Get(p1);
        T obj2 = Get(p2);

        Set(p1, obj2);
        Set(p2, obj1);

        return true;
    }

    public List<T> GetAllValue()
    {
        List<T> values = new List<T>();
        foreach (var item in Values())
        {
            values.Add(item);
        }

        return values;
    }

    public IEnumerable<T> Values()
    {
        for (int i = 0; i < _values.GetLength(0); i++)
        {
            for (int j = 0; j < _values.GetLength(1); j++)
            {
                yield return Get(new GridPosition(i, j));
            }
        }
    }

    public bool TryGet(GridPosition gridPosition, out T value)
    {
        value = default(T);
        if (!IsValid(gridPosition))
            return false;

        value = Get(gridPosition);
        return true;
    }

    public bool TrySet(GridPosition gridPosition, T value)
    {
        if (!IsValid(gridPosition)) 
            return false;

        Set(gridPosition, value);
        return true;
    }

    public T Get(GridPosition gridPosition)
        => _values[gridPosition.Column, gridPosition.Row];

    public void Set(GridPosition gridPosition, T value)
    {
        _values[gridPosition.Column, gridPosition.Row] = value;
        OnGridValueChanged?.Invoke(gridPosition);
    }

    public Vector3 GridToWorld(GridPosition gridPosition)
    {
        float x = (gridPosition.Column + 0.5f) * CellSize;
        float y = (gridPosition.Row + 0.5f) * CellSize;

        return new Vector3(x, y) + _translationVector;
    }

    public GridPosition WorldToGrid(Vector3 worldPosition)
    {
        worldPosition -= _translationVector;

        int x = Mathf.FloorToInt(worldPosition.x / CellSize);
        int y = Mathf.FloorToInt(worldPosition.y / CellSize);

        return new GridPosition(x, y);
    }

    public bool IsValid(GridPosition gridPosition)
        => 0 <= gridPosition.Column && gridPosition.Column < Columns && 0 <= gridPosition.Row && gridPosition.Row < Rows;

    public CustomizedGrid<T> Clone()
    {
        CustomizedGrid<T> cloneObj = new CustomizedGrid<T>(Columns, Rows, CellSize);
        

        return cloneObj;
    }
}
