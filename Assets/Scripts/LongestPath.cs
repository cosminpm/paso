using System.Collections.Generic;
using UnityEngine;

public class LongestPath : MonoBehaviour
{
    public int rows;
    public int columns;
    public int[] startCell;
    public HashSet<int[]> blockedCells = new HashSet<int[]>(new IntArrayEqualityComparer());

    private int[,] distances;
    private bool[,] visited;

    private void Start()
    {
        // Initialize distances and visited arrays
        distances = new int[rows, columns];
        visited = new bool[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                distances[i, j] = int.MinValue;
                visited[i, j] = false;
            }
        }

        // Start the DFS
        int longestPath = DFS(startCell);

        // Print the result
        Debug.Log("Longest path: " + longestPath);
    }

    private int DFS(int[] cell)
    {
        visited[cell[0], cell[1]] = true;
        int maxDistance = 0;

        foreach (int[] neighbor in GetNeighbors(cell))
        {
            if (!visited[neighbor[0], neighbor[1]] && !blockedCells.Contains(neighbor))
            {
                int distance = DFS(neighbor);
                maxDistance = Mathf.Max(maxDistance, distance);
            }
        }

        visited[cell[0], cell[1]] = false;
        distances[cell[0], cell[1]] = maxDistance + 1;

        return distances[cell[0], cell[1]];
    }

    private List<int[]> GetNeighbors(int[] cell)
    {
        List<int[]> neighbors = new List<int[]>();

        if (cell[0] > 0) // Left
        {
            neighbors.Add(new [] {cell[0], cell[1]});
        }
        if (cell[0] < rows - 1) // Right
        {
            neighbors.Add(new []{cell[0] + 1,cell[1]});
        }
        if (cell[1] > 0) // Down
        {
            neighbors.Add(new []{cell[0], cell[1] - 1});
        }
        if (cell[1] < columns - 1) // Up
        {
            neighbors.Add(new []{cell[0], cell[1] + 1});
        }

        return neighbors;
    }
}
