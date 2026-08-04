
public class GridObject : IOnGrid
{
    public GridPosition GridPosition { get; private set; }
    public GridPosition PreviousGrid {  get; private set; }
    public int ItemID {  get; private set; }

    public GridObject(int itemID)
    {
        ItemID = itemID;
    }

    public void SetID(int id)
    {
        ItemID = id;
    }

    public string GetDebugText()
    {
        return ItemID.ToString();
    }

    public void SetGridPosition(GridPosition gridPosition)
    {
        PreviousGrid = this.GridPosition;
        this.GridPosition = gridPosition;
    }
}
