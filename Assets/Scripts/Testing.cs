using System;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public int rows = 100;
    public int columns = 100;
    public float scaler = 0.15f;
    public float limiterPerlinNoise = 0.5f;
    private GameObject[,] cubes;


    private void Start()
    {
        GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGridColor();
    }

    private void GenerateGrid()
    {
        cubes = new GameObject[rows, columns];

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                int cellType = CalculateTypeCell(x, y);
                Vector3 position = new Vector3(x, 0, y);

                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = position;
                cube.transform.localScale = new Vector3(1f, 1f, 1f); // Adjust scale as needed

                // Store the cube reference
                cubes[x, y] = cube;
            }
        }
        
        // Update colors after generating the grid
        UpdateGridColor();
    }

    private void UpdateGridColor()
    {
        if (cubes != null)
        {
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < columns; y++)
                {
                    int cellType = CalculateTypeCell(x, y);
                    GameObject cube = cubes[x, y];

                    // Update the color based on cell type
                    if (cellType == 1)
                        cube.GetComponent<Renderer>().material.color = Color.yellow;
                    else
                        cube.GetComponent<Renderer>().material.color = Color.magenta;
                }
            }
        }
    }

    private void ClearGrid()
    {
        // Destroy all existing cubes
        if (cubes != null)
        {
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < columns; y++)
                {
                    Destroy(cubes[x, y]);
                }
            }
        }
    }

    private int CalculateTypeCell(int x, int y)
    {
        float val = Mathf.PerlinNoise((float)x / rows * scaler, (float)y / columns * scaler);

        if (val > limiterPerlinNoise)
            return 1;
        return 0;
    }
}
