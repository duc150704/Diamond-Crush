public class GridObject : ICloneable<GridObject>
{
    public GridPosition GridPosition { get; set; }
    public int ItemType {  get; set; }

    public GridObject(int itemType)
    {
        ItemType = itemType;
    }

    public string GetDebugText()
    {
        return ItemType.ToString();
    }

    public GridObject Clone()
    {
        return new GridObject(ItemType)
        {
            GridPosition = this.GridPosition,
        };
    }
}
