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
        longestPathListCells = _longestPath.FindLongestPath(_grid.startingPosition, _grid.poisonArrIntHashSet);
        Debug.Log("FINAL_RESULT:"+longestPathListCells.Count);
    }

    private void setDFSSize()
    {
        _longestPath.columns = _grid.columns;
        _longestPath.rows = _grid.rows;
    }
    
    
    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            _longestPath.DrawDebugVisited(longestPathListCells, _grid.gridCell);
        }
    }
}
