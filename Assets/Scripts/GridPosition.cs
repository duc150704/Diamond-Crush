using System;

public struct GridPosition
{
    public int Column;
    public int Row;

    public static GridPosition Zero = new GridPosition(0, 0);

    public GridPosition(int column, int row)
    {
        this.Column = column;
        this.Row = row;
    }

    public override string ToString()
    {
        return $"{Column}, {Row}";
    }

    public GridPosition GetUp()
    {
        return new GridPosition(Column, Row + 1);
    }

    public GridPosition GetLeft()
    {
        return new GridPosition(Column - 1, Row);
    }    
    
    public GridPosition GetDown()
    {
        return new GridPosition(Column, Row - 1);
    }    
    
    public GridPosition GetRight()
    {
        return new GridPosition(Column + 1, Row);
    }

    public static bool operator ==(GridPosition a, GridPosition b)
    {
        return a.Column == b.Column && a.Row == b.Row;
    }

    public static bool operator !=(GridPosition a, GridPosition b)
    {
        return a.Column != b.Column || a.Row != b.Row;
    }

    public static GridPosition operator +(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.Column + b.Column, a.Row + b.Row);
    }

    public override bool Equals(object obj)
    {
        return obj is GridPosition other && Row == other.Row && Column == other.Column;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Column, Row);
    }

    public static GridPosition operator -(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.Column - b.Column, a.Row - b.Row);
    }
        
}
