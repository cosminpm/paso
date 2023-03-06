using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using Random = UnityEngine.Random;

public class Grid : MonoBehaviour
{
    public int rows;
    public int columns;
    public float limiterPerlinNoise = 0.5f;
    public float scaler = 0.15f;

    public GameObject cellPrefab;
    public List<GameObject> desertGameObjectList;
    public List<GameObject> poisonDesertGameObjectList;
    public GameObject forestGameObject;

    private GameObject[,] _gridCell;
    private HashSet<GameObject> _poisonGoHashSet;

    private HashSet<GameObject> _desertGoHashSet;
    private HashSet<int[]> _desertArrIntPositions;

    // TODO: To be done
    private HashSet<GameObject> _forestGoHashSet;
    private Astronaut _astronautScript;

    public enum CellType
    {
        Desert,
        DesertPoison,
        Forest
    }

    private void Update()
    {
        MoveAstronaut();
    }

    void Start()
    {
        InstantiateGrid();
        InstantiateAstronaut();
    }


    private GameObject GetRandomGameObjectFromList(List<GameObject> gameObjects)
    {
        int n = Random.Range(0, gameObjects.Count);
        return gameObjects[n];
    }

    private GameObject GetRandomGameObjectFromSet(HashSet<GameObject> gameObjects)
    {
        List<GameObject> output = new List<GameObject>();
        output.AddRange(gameObjects);
        return GetRandomGameObjectFromList(output);
    }


    private CellType CalculateTypeCell(int x, int y)
    {
        // Divide by rows and columns as PerlinNoise function needs float values
        float val = Mathf.PerlinNoise((float)x / rows * scaler, (float)y / columns * scaler);

        if (val > limiterPerlinNoise)
            return CellType.DesertPoison;
        return CellType.Desert;
    }

    private GameObject GetGameObjectBasedOnCellType(CellType cellType)
    {
        if (cellType == CellType.Desert)
            return GetRandomGameObjectFromList(desertGameObjectList);
        if (cellType == CellType.DesertPoison)
            return GetRandomGameObjectFromList(poisonDesertGameObjectList);

        // TODO: It should never arrive here
        return null;
    }

    void InstantiateGrid()
    {
        _gridCell = new GameObject[rows, columns];
        _desertGoHashSet = new HashSet<GameObject>();
        _poisonGoHashSet = new HashSet<GameObject>();
        _desertArrIntPositions = new HashSet<int[]>();
        
        Transform cube = cellPrefab.transform.Find("Cube");
        Vector3 cellSize = cube.GetComponent<Renderer>().bounds.size;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector3 cellPosition = new Vector3(i * cellSize.x, 0, j * cellSize.z);
                CellType ct = CalculateTypeCell(i, j);
                GameObject parent = new GameObject();
                parent.transform.position = cellPosition;
                parent.name = $"Parent ({i}, {j})";

                GameObject go = Instantiate(GetGameObjectBasedOnCellType(ct), parent.transform.position,
                    Quaternion.identity,
                    parent.transform);
                go.name = $"Cell ({i}, {j})";
                parent.transform.parent = GameObject.Find("Grid").transform;

                parent.AddComponent<Cell>();
                parent.GetComponent<Cell>().CreateCell(cellPosition, i, j, parent, ct);

                if (ct == CellType.DesertPoison)
                    _poisonGoHashSet.Add(parent);
                else
                {
                    _desertGoHashSet.Add(parent);
                    _desertArrIntPositions.Add(new[] { i, j });
                }


                _gridCell[i, j] = parent;
            }
        }
    }


    private int[,] GetRandomCellFromGrid()
    {
        int x = Random.Range(0, rows);
        int y = Random.Range(0, columns);
        return new int[x, y];
    }

    void InstantiateAstronaut()
    {
        GameObject startCellAstronaut = GetRandomGameObjectFromSet(_desertGoHashSet);
        Cell cellStart = startCellAstronaut.GetComponent<Cell>();

        GameObject astronautGameObject = GameObject.Find("Astronaut");
        Astronaut astronautScript = astronautGameObject.AddComponent<Astronaut>();
        astronautScript.InstantiateAstronaut(cellStart.GetX(), cellStart.GetY(), cellStart.GetPosition());
        _astronautScript = astronautScript;
    }

    private bool IsPositionIsInsideGrid(int[] position)
    {
        if (position[0] >= rows
            || position[1] >= columns
            || position[0] < 0
            || position[1] < 0)
            return false;
        return true;
    }

    private bool IsPositionGoodDesert(int[] position)
    {
        if (_desertArrIntPositions.Contains(position))
        {
            Debug.Log("Me he muerto");
        }

        return false;
    }

    private void MoveAstronaut()
    {
        // Check input 
        int[] positionDesired = { -1, -1 };
        if (Input.GetKeyDown("up"))
            positionDesired = new[] { _astronautScript.GetX(), _astronautScript.GetY() + 1 };

        else if (Input.GetKeyDown("down"))
            positionDesired = new[] { _astronautScript.GetX(), _astronautScript.GetY() - 1 };

        else if (Input.GetKeyDown("right"))
            positionDesired = new[] { _astronautScript.GetX() + 1, _astronautScript.GetY() };

        else if (Input.GetKeyDown("left"))
            positionDesired = new[] { _astronautScript.GetX() - 1, _astronautScript.GetY() };

        // Check if position is a position inside the grid
        if (IsPositionIsInsideGrid(positionDesired))
        {
            GameObject parent = _gridCell[positionDesired[0], positionDesired[1]];
            Cell cell = parent.GetComponent<Cell>();
            _astronautScript.SetPosition(cell.GetPosition());
            _astronautScript.SetX(positionDesired[0]);
            _astronautScript.SetY(positionDesired[1]);
            GameObject desertPrefab = parent.transform.GetChild(0).gameObject;
            Destroy(desertPrefab);
            GameObject forest = Instantiate(forestGameObject, parent.transform.position, Quaternion.identity,
                parent.transform);
            forest.transform.parent = parent.transform;
            IsPositionGoodDesert(positionDesired);
            _desertArrIntPositions.Remove(positionDesired);
            
        }
    }
}