using System.Collections.Generic;

public static class ShapeDetector 
{
    public static void DetectMinPivot(HashSet<GridPosition> positions, out int xMin, out int yMin)
    {
        xMin = int.MaxValue;
        yMin = int.MaxValue;

        foreach (var pos in positions)
        {
            if (pos.Row < yMin)
                yMin = pos.Row;
            if (pos.Column < xMin)
                xMin = pos.Column;
        }
    }

    public static Shape DetectShape(HashSet<GridPosition> positions, int xMin, int yMin)
    {
        if (positions.Count == 3)
            return Shape.Line3;

        if (positions.Count == 4)
            return Shape.Line4;

        HashSet<GridPosition> tmp = new();
        foreach (var item in positions)
        {
            tmp.Add(new GridPosition(item.Column - xMin, item.Row - yMin));
        }

        if (tmp.Count == 5)
        {
            if (Compare(tmp, ResultPatterns.Line5))
                return Shape.Line5;
            if (Compare(tmp, ResultPatterns.Cross))
                return Shape.Cross;
            if (Compare(tmp, ResultPatterns.SquareConner))
                return Shape.SquareConner;

            return Shape.TShape;
        }

        return Shape.ChuaTinhDen;
    }

    private static bool Compare(HashSet<GridPosition> positions, List<HashSet<GridPosition>> patterns)
    {
        foreach (var item in patterns)
        {
            if(Compare(positions, item))
                return true;
        }
        return false;
    }

    private static bool Compare(HashSet<GridPosition> firstSet, HashSet<GridPosition> secondSet)
    {
        foreach (var item in firstSet)
        {
            if (!secondSet.Contains(item))
                return false;
        }
        return true;
    }
}
