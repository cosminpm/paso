using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.LevelTypes;
using Unity.VisualScripting;

public class AllCellLevel : LevelCell
{
    private LongestPath _longestPath;
    private List<int[]> _longestPathListCells;


    public AllCellLevel(Grid grid, LongestPath longestPath) : base(grid)
    {
        _longestPath = longestPath;
    }

    public override void GridSpecificLevel()
    {
        grid.SetRandomLimiterPerlinNoise(.5f, .75f);
        grid.SetRandomScaler(2f, 10f);
        grid.SetRandomColumnsAndRows(3, 6);
    }

    public override void CreateLevelSpecific()
    {
        SetDFSSize();
        _longestPath.InitializeDFS();
        _longestPathListCells = _longestPath.FindLongestPath(grid.startingPosition, grid.poisonArrIntHashSet);
        TransformUnusedDesertIntoPoison(_longestPathListCells);
        grid.CreateFinalCellPosition(_longestPathListCells.Last());
    }
    
    private void SetDFSSize()
    {
        _longestPath.columns = grid.columns;
        _longestPath.rows = grid.rows;
    }
    
    private void TransformUnusedDesertIntoPoison(List<int[]> usedInPath)
    {
        HashSet<int[]> hashUsedInPath = new HashSet<int[]>(new IntArrayEqualityComparer());
        hashUsedInPath.AddRange(usedInPath);
        IEnumerable<int[]> difference =
            grid.desertArrIntHashSet.Except(hashUsedInPath, new IntArrayEqualityComparer());
        List<int[]> result = difference.ToList();

        foreach (var cell in result)
        {
            grid.TransformIntoPoison(cell[0], cell[1]);
            grid.desertArrIntHashSet.Remove(cell);
        }
    }
    
}