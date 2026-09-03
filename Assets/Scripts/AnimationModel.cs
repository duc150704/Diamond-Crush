using System.Collections.Generic;

public class AnimationModel
{
    public CustomizedGrid<GridObject> Grid { get; private set; }
    public List<DiamondSO> ObjectData { get; private set; }

    public AnimationModel(CustomizedGrid<GridObject> grid, List<DiamondSO> data) 
    { 
        Grid = grid;
        ObjectData = data;
    }
}
