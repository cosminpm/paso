using System.Collections.Generic;
using DefaultNamespace.LevelTypes;

public class MaximizeCellLevel : LevelCell
{
    private List<int[]> _longestPathListCells;

    public MaximizeCellLevel(Grid grid) : base(grid)
    {
    }

    public override void GridSpecificLevel()
    {
        grid.SetRandomLimiterPerlinNoise(.5f, .75f);
        grid.SetRandomScaler(2f, 10f);
        grid.SetRandomColumnsAndRows(10, 12);
    }

    public override void CreateLevelSpecific()
    {
        int[] endPos = GetFinalCellInMaximize();
        grid.CreateFinalCellPosition(endPos);
    }

    int[] GetFinalCellInMaximize()
    {
        int[] endPos = null;

        List<int[]> positionsEnd = new List<int[]>();

        positionsEnd.Add(new int[] {grid.startingPosition[0] + 2, grid.startingPosition[1]});
        positionsEnd.Add(new int[] {grid.startingPosition[0] - 2, grid.startingPosition[1]});
        positionsEnd.Add(new int[] {grid.startingPosition[0], grid.startingPosition[1] + 2});
        positionsEnd.Add(new int[] {grid.startingPosition[0], grid.startingPosition[1] - 2});

        foreach (var pos in positionsEnd)
        {
            if (grid.IsPositionIsInsideGrid(pos))
            {
                endPos = pos;
                break;
            }
        }

        return endPos;
    }
}