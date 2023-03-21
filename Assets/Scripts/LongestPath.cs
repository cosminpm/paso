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
                _distances[i, j] = int.MinValue;
                _visited[i, j] = false;
            }
        }
    }

    public int DFS(int[] cell, HashSet<int[]> posionCells)
    {
        _visited[cell[0], cell[1]] = true;
        int maxDistance = 0;

        foreach (int[] neighbor in GetNeighbors(cell, posionCells))
        {
            if (!_visited[neighbor[0], neighbor[1]])
            {
                int distance = DFS(neighbor, posionCells);
                maxDistance = Mathf.Max(maxDistance, distance);
            }
        }

        _visited[cell[0], cell[1]] = false;
        _distances[cell[0], cell[1]] = Mathf.Max(_distances[cell[0], cell[1]],maxDistance + 1);

        return maxDistance +1;
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

    private int[] NextCellInNeighbours(List<int[]> neighbours, HashSet<int[]> alreadyVisited)
    {
        int max = -1;
        int[] result = null;
        for (int i = 0; i < neighbours.Count; i++)
        {
            int[] neighbour = neighbours[i];
            if (!alreadyVisited.Contains(neighbour) && _distances[neighbour[0],neighbour[1]] > max)
            {
                max = _distances[neighbour[0], neighbour[1]];
                result = new[] {neighbour[0], neighbour[1]};
            }
        }
        return result;
    }

    public List<int[]> GetLongestPath(HashSet<int[]> poisonCells, int[] starCell)
    {
        int[] currentCell = {starCell[0], starCell[1]};
        int currentWeight = _distances[currentCell[0], currentCell[1]];
        List<int[]> longestPath = new List<int[]>();

        HashSet<int[]> alreadyVisited = new HashSet<int[]>(new IntArrayEqualityComparer());

        while (currentWeight > 0)
        {
            List<int[]> neighbors = GetNeighbors(currentCell, poisonCells);
            int [] aux  = NextCellInNeighbours(neighbors, alreadyVisited);
            alreadyVisited.Add(aux);
            if (aux == null) 
                break;
            currentCell = new[] {aux[0], aux[1]};
            currentWeight -= 1;
            longestPath.Add(currentCell);
        }
        return longestPath;
    }
    
    
    public void DrawDebugVisited(GameObject[,] grid, List<int[]> longestPath)
    {
        foreach (var cell in longestPath)
        {
            Gizmos.DrawWireCube(grid[cell[0], cell[1]].transform.position, new Vector3(.5f, .5f, .5f));
            Handles.Label(grid[cell[0], cell[1]].transform.position, _distances[cell[0], cell[1]].ToString());
        }
    
        Gizmos.color = Color.red;
        for (int i = 0; i < _distances.GetLength(0); i++)
        {
           
            for (int j = 0; j < _distances.GetLength(1); j++)
            {
                Gizmos.DrawWireCube(grid[i,j].transform.position, new Vector3(.25f, .25f, .25f));
                Handles.Label(grid[i,j].transform.position, _distances[i,j].ToString());
            } 
        }
    }
}