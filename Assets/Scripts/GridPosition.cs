
public struct GridPosition
{
    public int x;
    public int y;

    public GridPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return $"{x}, {y}";
    }

    public GridPosition GetNeighbor(DragDirection direction)
    {
        switch (direction)
        {
            case DragDirection.Left:
                return GetLeft();
            case DragDirection.Right:
                return GetRight();
            case DragDirection.Down:
                return GetDown();
            case DragDirection.Up:
                return GetUp();
            default:
                return this;
        }
    }

    public GridPosition GetUp()
    {
        return new GridPosition(x, y + 1);
    }

    public GridPosition GetLeft()
    {
        return new GridPosition(x - 1, y);
    }    
    
    public GridPosition GetDown()
    {
        return new GridPosition(x, y - 1);
    }    
    
    public GridPosition GetRight()
    {
        return new GridPosition(x + 1, y);
    }
}
