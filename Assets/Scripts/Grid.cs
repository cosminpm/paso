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
    public List<GameObject> forestGameObjectList;

    private GameObject[,] _gridCell;

    private int[] _startingPosition;
    private int[] _finishPosition;

    private HashSet<int[]> _desertArrIntHashSet = new HashSet<int[]>(new IntArrayEqualityComparer());
    private HashSet<int[]> _poisonArrIntHashSet = new HashSet<int[]>(new IntArrayEqualityComparer());
    private HashSet<int[]> _forestArrIntHashSet = new HashSet<int[]>(new IntArrayEqualityComparer());


    private Astronaut _astronautScript;

    private Dictionary<CellType, List<GameObject>> _dictCellTypeListGameObjects;

    public enum CellType
    {
        Desert,
        DesertPoison,
        Forest
    }


    private GameObject GetRandomGridCell()
    {
        int randomRow = Random.Range(0, rows);
        int randomCol = Random.Range(0, columns);
        return _gridCell[randomRow, randomCol];
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


    private T GetRandomFromList<T>(List<T> objects)
    {
        int n = Random.Range(0, objects.Count);
        return objects[n];
    }

    private T GetRandomFromSet<T>(HashSet<T> objects)
    {
        int randomIndex = Random.Range(0, objects.Count);
        using (var enumerator = objects.GetEnumerator())
        {
            for (int i = 0; i <= randomIndex; i++)
            {
                enumerator.MoveNext();
            }

            return enumerator.Current;
        }
    }


    private CellType CalculateTypeCell(int x, int y)
    {
        // Divide by rows and columns as PerlinNoise function needs float values
        float val = Mathf.PerlinNoise((float) x / rows * scaler, (float) y / columns * scaler);

        if (val > limiterPerlinNoise)
            return CellType.DesertPoison;
        return CellType.Desert;
    }

    private GameObject GetGameObjectBasedOnCellType(CellType cellType)
    {
        if (cellType == CellType.Desert)
            return GetRandomFromList(desertGameObjectList);
        else if (cellType == CellType.DesertPoison)
            return GetRandomFromList(poisonDesertGameObjectList);

        // TODO: It should never arrive here
        return null;
    }

    void InstantiateGrid()
    {
        _dictCellTypeListGameObjects = new Dictionary<CellType, List<GameObject>>()
        {
            {CellType.Desert, desertGameObjectList},
            {CellType.Forest, forestGameObjectList},
            {CellType.DesertPoison, poisonDesertGameObjectList}
        };


        _gridCell = new GameObject[rows, columns];
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
                    _poisonArrIntHashSet.Add(new[] {i, j});
                else
                {
                    _desertArrIntHashSet.Add(new[] {i, j});
                }

                _gridCell[i, j] = parent;
            }
        }
    }


    void InstantiateAstronaut()
    {
        int[] pos = GetRandomFromSet(_desertArrIntHashSet);
        GameObject startCellAstronaut = _gridCell[pos[0], pos[1]];
        TransformIntoForest(pos[0],pos[1]);
        Cell cellStart = startCellAstronaut.GetComponent<Cell>();

        _startingPosition = new[] {cellStart.GetX(), cellStart.GetY()};

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
        if (_desertArrIntHashSet.Contains(position))
        {
            Debug.Log("Me he muerto");
        }

        return false;
    }

    private void MoveAstronaut()
    {
        // Check input 
        int[] positionDesired = {-5, -5};
        if (Input.GetKeyDown("up"))
            positionDesired = new[] {_astronautScript.GetX(), _astronautScript.GetY() + 1};

        else if (Input.GetKeyDown("down"))
            positionDesired = new[] {_astronautScript.GetX(), _astronautScript.GetY() - 1};

        else if (Input.GetKeyDown("right"))
            positionDesired = new[] {_astronautScript.GetX() + 1, _astronautScript.GetY()};

        else if (Input.GetKeyDown("left"))
            positionDesired = new[] {_astronautScript.GetX() - 1, _astronautScript.GetY()};

        // Check if position is a position inside the grid
        if (IsPositionIsInsideGrid(positionDesired) &&
            !_poisonArrIntHashSet.Contains(positionDesired) &&
            !_forestArrIntHashSet.Contains(positionDesired))
        {
            GameObject parent = _gridCell[positionDesired[0], positionDesired[1]];

            _astronautScript.SetAllPositionAstronaut(positionDesired[0], positionDesired[1],
                parent.GetComponent<Cell>().GetPosition());

            TransformIntoForest(positionDesired[0], positionDesired[1]);
            _desertArrIntHashSet.Remove(positionDesired);
        }
        else if (positionDesired[0] != -5 && positionDesired[1] != -5 &&
                 (_poisonArrIntHashSet.Contains(positionDesired) || _forestArrIntHashSet.Contains(positionDesired)))
        {
            Debug.Log("I AM STARTING");
            Cell cell = _gridCell[_startingPosition[0], _startingPosition[1]].GetComponent<Cell>();
            _astronautScript.SetAllPositionAstronaut(_startingPosition[0], _startingPosition[1], cell.GetPosition());
            ConvertAllForestIntoDesert();
            
            TransformIntoForest(_startingPosition[0],_startingPosition[1]);
            Debug.Log(_forestArrIntHashSet);
            _desertArrIntHashSet.Remove(_startingPosition);
        }
    }

    private void ConvertAllForestIntoDesert()
    {
        foreach (var forest in _forestArrIntHashSet)
            TransformIntoDesert(forest[0], forest[1]);
        _forestArrIntHashSet.Clear();
    }

    private GameObject TransformTerrainIntoAnother(int x, int y, CellType cellType)
    {
        GameObject parent = _gridCell[x, y];
        foreach (Transform child in parent.transform)
        {
            // Destroy the child GameObject
            Destroy(child.gameObject);
        }
        
        GameObject go = GetRandomFromList(_dictCellTypeListGameObjects[cellType]);

        GameObject cellGameObject = Instantiate(go, parent.transform.position, Quaternion.identity, parent.transform);
        cellGameObject.transform.parent = parent.transform;
        return parent;
    }

    private void TransformIntoForest(int x, int y)
    {
        TransformTerrainIntoAnother(x, y, CellType.Forest);
        _forestArrIntHashSet.Add(new[] {x, y});
    }

    private void TransformIntoDesert(int x, int y)
    {
        TransformTerrainIntoAnother(x, y, CellType.Desert);
        _desertArrIntHashSet.Add(new[] {x, y});
    }
}