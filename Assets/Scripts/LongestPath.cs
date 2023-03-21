using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LongestPath : MonoBehaviour
{
    public int rows;
    public int columns;

    private int[,] _distances;
    private bool[,] _visited;

    public void InitializeDFS()
    {
        // Initialize distances and visited arrays
        _distances = new int[rows, columns];
        _visited = new bool[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                _distances[i, j] = int.MaxValue;
                _visited[i, j] = false;
            }
        }
    }

    public List<int[]> FindLongestPath(int[] startPosition, HashSet<int[]> posisonCells) {
        List<int[]> longestPath = new List<int[]>();
        HashSet<int[]> visited = new HashSet<int[]>(new IntArrayEqualityComparer());
        List<int[]> currentPath = new List<int[]>();
        FindLongestPathHelper(startPosition, visited, currentPath, posisonCells, ref longestPath);
        return longestPath;
    }

    private void FindLongestPathHelper(int[] currentPosition, HashSet<int[]> visited, 
        List<int[]> currentPath, HashSet<int[]> poisonCells,  ref List<int[]> longestPath) {
        
        visited.Add(currentPosition);
        currentPath.Add(currentPosition);

        List<int[]> possibleMoves = GetNeighbors(currentPosition, poisonCells);
        List<int[]> validMoves = new List<int[]>();
        foreach (int[] move in possibleMoves) {
            if (!visited.Contains(move) && !poisonCells.Contains(move)) {
                validMoves.Add(move);
            }
        }

        if (validMoves.Count == 0 ) {
            if (currentPath.Count > longestPath.Count)
            {
                longestPath.Clear();
                for (int i = 0; i < currentPath.Count; i++)
                    longestPath.Add(new []{currentPath[i][0],currentPath[i][1]});
                
            }
        } else {
            foreach (int[] move in validMoves) {
                FindLongestPathHelper(move, visited, currentPath, poisonCells, ref longestPath);
            }
        }

        visited.Remove(currentPosition);
        currentPath.RemoveAt(currentPath.Count - 1);
    }
    private bool CheckCellGoodNeighbour(int[] cell, HashSet<int[]> posionCells)
    {
        if (cell[0] >= 0 &&
            cell[1] >= 0 &&
            cell[0] < rows &&
            cell[1] < columns &&
            !posionCells.Contains(cell))
            return true;
        return false;
    }

    private List<int[]> GetNeighbors(int[] cell, HashSet<int[]> posionCells)
    {
        List<int[]> neighbors = new List<int[]>();
        if (CheckCellGoodNeighbour(new[] {cell[0] - 1, cell[1]}, posionCells)) // Left
        {
            neighbors.Add(new[] {cell[0] - 1, cell[1]});
        }

        if (CheckCellGoodNeighbour(new[] {cell[0] + 1, cell[1]}, posionCells)) // Right
        {
            neighbors.Add(new[] {cell[0] + 1, cell[1]});
        }

        if (CheckCellGoodNeighbour(new[] {cell[0], cell[1] - 1}, posionCells)) // Up
        {
            neighbors.Add(new[] {cell[0], cell[1] - 1});
        }

        if (CheckCellGoodNeighbour(new[] {cell[0], cell[1] + 1}, posionCells)) // Down
        {
            neighbors.Add(new[] {cell[0], cell[1] + 1});
        }
        return neighbors;
    }


    public void DrawDebugVisited(List<int[]> longestPath, GameObject[,] grid)
    {
        for (int i = 0; i < longestPath.Count; i++)
        {
            Gizmos.DrawWireCube(grid[longestPath[i][0],longestPath[i][1]].transform.position,new Vector3(.5f,.5f,.5f));
            Handles.Label(grid[longestPath[i][0],longestPath[i][1]].transform.position, i.ToString());
        }
    }
}