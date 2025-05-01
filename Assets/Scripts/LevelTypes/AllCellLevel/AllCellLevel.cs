using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DefaultNamespace.LevelTypes;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class AllCellLevel : LevelCell
{   
    public int downLimiter = 5;
    public int upLimiter = 5;
    
    private LongestPath _longestPath;
    private List<int[]> _longestPathListCells;

    public AllCellLevel(Grid grid) : base(grid)
    {
        _longestPath =  GameObject.Find("Grid").GetComponent<LongestPath>();
    }

    public override void GridSpecificLevel()
    {
        grid.SetRandomLimiterPerlinNoise(.5f, .75f);
        grid.SetRandomScaler(2f, 10f);
        SetRandomColumnsAndRows();
    }

    public override bool EndCondition()
    {
        return grid.desertArrIntHashSet.Count == 0;
    }

    public override void CreateLevelSpecific()
    {
        // Start measuring time
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        SetDFSSize();
        _longestPath.InitializeDFS();
        _longestPathListCells = _longestPath.FindLongestPath(grid.startingPosition, grid.poisonArrIntHashSet);
        Debug.Log("CreateLevelSpecific execution time: " + stopwatch.ElapsedMilliseconds + " ms");
        TransformUnusedDesertIntoPoison(_longestPathListCells);
        grid.CreateFinalCellPosition(_longestPathListCells.Last());
        stopwatch.Stop();
        Debug.Log("CreateLevelSpecific execution time: " + stopwatch.ElapsedMilliseconds + " ms");


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
    
    public void SetRandomColumnsAndRows()
    {
        grid.columns = Random.Range(downLimiter, upLimiter);
        grid.rows = Random.Range(downLimiter, upLimiter);
    }
    
}