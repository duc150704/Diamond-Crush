
public interface IOnGrid 
{
    public GridPosition GridPosition { get; }
    public void SetGridPosition(GridPosition gridPosition);
    public string GetDebugText();
}
