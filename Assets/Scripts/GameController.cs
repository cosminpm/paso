using System;
using System.Collections.Generic;
using UnityEngine;
public class GameController: MonoBehaviour
{
    private LongestPath _longestPath;
    private Grid _grid;
    public bool drawGizmos;
    private List<int[]> longestPathListCells;

    private void Start()
    {

        _grid = GameObject.Find("Grid").GetComponent<Grid>();
        _longestPath = GameObject.Find("Grid").GetComponent<LongestPath>();
        
        _grid.InstantiateDictionaryCellType();
        _grid.InstantiateGrid();
        _grid.InstantiateAstronaut();
        setDFSSize();
        _longestPath.InitializeDFS();
        CreatePathForGrid();
        longestPathListCells = new List<int[]>(_longestPath.GetLongestPath(_grid.poisonArrIntHashSet, _grid.startingPosition));

    }

    private void setDFSSize()
    {
        _longestPath.columns = _grid.columns;
        _longestPath.rows = _grid.rows;
    }
    
    private void CreatePathForGrid()
    {
        int longestPath = 0;
        int total_distance = _longestPath.DFS(_grid.startingPosition, _grid.poisonArrIntHashSet);
    }

    
    
    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            _longestPath.DrawDebugVisited(_grid.gridCell, longestPathListCells);
        }
    }
}
