using System.Collections.Generic;

public static class ResultPatterns
{
    public static readonly List<HashSet<GridPosition>> Line5 = 
        new List<HashSet<GridPosition>>() 
        {
            new HashSet<GridPosition>() //Vertical
            {
                new GridPosition(0,0),
                new GridPosition(1,0),
                new GridPosition(2,0),
                new GridPosition(3,0),
                new GridPosition(4,0),
            },

            new HashSet<GridPosition>() //Horizontal
            {
                new GridPosition(0,0),
                new GridPosition(0,1),
                new GridPosition(0,2),
                new GridPosition(0,3),
                new GridPosition(0,4),
            }
        };

    public static readonly List<HashSet<GridPosition>> SquareConner =
        new List<HashSet<GridPosition>>()
        {
            new HashSet<GridPosition>() //LeftUp
            {
                new GridPosition(0,0),
                new GridPosition(1,0),
                new GridPosition(2,0),
                new GridPosition(0,1),
                new GridPosition(0,2),
            },

            new HashSet<GridPosition>() //LeftDown
            {
                new GridPosition(0,0),
                new GridPosition(0,1),
                new GridPosition(0,2),
                new GridPosition(1,2),
                new GridPosition(2,2),
            },

            new HashSet<GridPosition>() //RightUp
            {
                new GridPosition(2,0),
                new GridPosition(2,1),
                new GridPosition(2,2),
                new GridPosition(1,2),
                new GridPosition(0,2),
            },

            new HashSet<GridPosition>() //RightDown
            {
                new GridPosition(0,0),
                new GridPosition(1,0),
                new GridPosition(2,0),
                new GridPosition(2,1),
                new GridPosition(2,2),
            }
        };

    public static readonly List<HashSet<GridPosition>> Cross =
        new List<HashSet<GridPosition>>()
        {
            new HashSet<GridPosition>()
            {
                new GridPosition(1,0),
                new GridPosition(1,1),
                new GridPosition(0,1),
                new GridPosition(2,1),
                new GridPosition(1,2),
            }
        };

    public static readonly List<HashSet<GridPosition>> TShape =
        new List<HashSet<GridPosition>>()
        {
            new HashSet<GridPosition>() //Up
            {
                new GridPosition(1,0),
                new GridPosition(1,1),
                new GridPosition(1,2),
                new GridPosition(0,2),
                new GridPosition(2,2),
            },            
            
            new HashSet<GridPosition>() //Right
            {
                new GridPosition(2,0),
                new GridPosition(2,1),
                new GridPosition(2,2),
                new GridPosition(0,1),
                new GridPosition(1,1),
            },            
            
            new HashSet<GridPosition>() //Down
            {
                new GridPosition(0,0),
                new GridPosition(1,0),
                new GridPosition(2,0),
                new GridPosition(1,1),
                new GridPosition(1,2),
            },            
            
            new HashSet<GridPosition>() //Left
            {
                new GridPosition(0,0),
                new GridPosition(0,1),
                new GridPosition(0,2),
                new GridPosition(1,1),
                new GridPosition(2,1),
            }
        };
}
