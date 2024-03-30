using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class AllCellLevel
{
    private LongestPath _longestPath;
    private Grid _grid;
    private List<int[]> _longestPathListCells;


    public AllCellLevel(Grid grid, LongestPath longestPath)
    {
        _grid = grid;
        _longestPath = longestPath;
    }
    
    public void CreateStepAllLevel()
    {
        SetDFSSize();
        _longestPath.InitializeDFS();
        _longestPathListCells = _longestPath.FindLongestPath(_grid.startingPosition, _grid.poisonArrIntHashSet);
        TransformUnusedDesertIntoPoison(_longestPathListCells);
        _grid.CreateFinalCellPosition(_longestPathListCells.Last());
    }
    
    private void SetDFSSize()
    {
        _longestPath.columns = _grid.columns;
        _longestPath.rows = _grid.rows;
    }
    
    private void TransformUnusedDesertIntoPoison(List<int[]> usedInPath)
    {
        HashSet<int[]> hashUsedInPath = new HashSet<int[]>(new IntArrayEqualityComparer());
        hashUsedInPath.AddRange(usedInPath);
        IEnumerable<int[]> difference =
            _grid.desertArrIntHashSet.Except(hashUsedInPath, new IntArrayEqualityComparer());
        List<int[]> result = difference.ToList();

        foreach (var cell in result)
        {
            _grid.TransformIntoPoison(cell[0], cell[1]);
            _grid.desertArrIntHashSet.Remove(cell);
        }
    }
    
}