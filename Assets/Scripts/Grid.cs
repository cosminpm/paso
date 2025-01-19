using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
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
    public List<GameObject> finalGameObjectList;
    
    public GameObject emptyGameObject;

    public GameObject[,] gridCell;

    public int[] startingPosition ;
    [FormerlySerializedAs("heart_position")] public int[] heartPosition = {-1, -1};

    private int[] _finalPosition =  {-1, -1};

    public HashSet<int[]> desertArrIntHashSet = new (new IntArrayEqualityComparer());
    public HashSet<int[]> poisonArrIntHashSet = new (new IntArrayEqualityComparer());
    private HashSet<int[]> _forestArrIntHashSet = new (new IntArrayEqualityComparer());
    public HashSet<int[]> emptyArrIntsHashSet = new (new IntArrayEqualityComparer());


    public GameObject heartLife;
    public SoundManager soundManager;
    public GameController gameController;
    public AstronautController astronautController;
    private Dictionary<CellType, List<GameObject>> _dictCellTypeListGameObjects;

    private Func<bool> _endCondition;

    public Vector3 sizeOfCell;

    public void SetRandomLimiterPerlinNoise(float downLimiter, float upLimiter)
    {
        float randomValue = Random.Range(downLimiter, upLimiter);
        limiterPerlinNoise = randomValue;
    }

    public void SetRandomScaler(float downLimiter, float upLimiter)
    {
        float randomValue = Random.Range(downLimiter, upLimiter);
        scaler = randomValue;
    }

    public void SetEndCondition(Func<bool> endCondition)
    {
        _endCondition = endCondition;
    }

    public void SetRandomColumnsAndRows(int downLimiter, int upLimiter)
    {
        columns = Random.Range(downLimiter, upLimiter);
        rows = Random.Range(downLimiter, upLimiter);
    }

    public enum CellType
    {
        Desert,
        DesertPoison,
        Forest,
        Final,
        Empty,
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
        float val = Mathf.PerlinNoise((float) x / rows * scaler, (float) y / columns * scaler);

        if (val > limiterPerlinNoise)
        {
            if (Random.Range(0, 2) == 1)
                return CellType.DesertPoison;
            return CellType.Empty;
        }

        if (val <= limiterPerlinNoise)
            return CellType.Desert;
        Debug.LogError("ERROR: val has not correct value");
        return CellType.Desert;
    }

    private GameObject GetGameObjectBasedOnCellType(CellType cellType)
    {
        if (cellType == CellType.Desert)
            return GetRandomFromList(desertGameObjectList);
        if (cellType == CellType.DesertPoison)
            return GetRandomFromList(poisonDesertGameObjectList);
        if (cellType == CellType.Empty)
            return emptyGameObject;
        return null;
    }

    public void InstantiateDictionaryCellType()
    {
        _dictCellTypeListGameObjects = new Dictionary<CellType, List<GameObject>>
        {
            {CellType.Desert, desertGameObjectList},
            {CellType.Forest, forestGameObjectList},
            {CellType.DesertPoison, poisonDesertGameObjectList},
            {CellType.Final, finalGameObjectList}
        };
    }

    public void InstantiateGrid()
    {
        sizeOfCell = GetRandomFromList(desertGameObjectList).transform.GetChild(0).GetComponent<Renderer>().bounds.size;

        gridCell = new GameObject[rows, columns];
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
                parent.GetComponent<Cell>().CreateCell(cellPosition, i, j);

                if (ct == CellType.DesertPoison)
                    poisonArrIntHashSet.Add(new[] {i, j});
                if (ct == CellType.Empty)
                {
                    emptyArrIntsHashSet.Add(new[] {i, j});
                    // Needs to be added so the Find longest path does not use this cell
                    poisonArrIntHashSet.Add(new[] {i, j});
                }

                else
                {
                    desertArrIntHashSet.Add(new[] {i, j});
                }

                gridCell[i, j] = parent;
            }
        }
    }


    public void CreateFinalCellPosition(int[] pos)
    {
        _finalPosition = pos;
        TransformIntoFinalCell(pos[0], pos[1]);
        desertArrIntHashSet.Remove(pos);
    }


    public void InstantiateAstronaut()
    {
        int[] pos = GetRandomFromSet(desertArrIntHashSet);
        GameObject startCellAstronaut = gridCell[pos[0], pos[1]];
        TransformIntoForest(pos[0], pos[1]);
        Cell cellStart = startCellAstronaut.GetComponent<Cell>();

        startingPosition = new[] {cellStart.GetX(), cellStart.GetY()};
        desertArrIntHashSet.Remove(startingPosition);
        _forestArrIntHashSet.Add(startingPosition);
        astronautController.InstantiateAstronaut(cellStart.GetX(), cellStart.GetY(), cellStart.GetPosition());
    }

    public bool IsPositionAvailable(int[] position)
    {
        if (!IsPositionInsideGrid(position)
            || emptyArrIntsHashSet.Contains(new[] {position[0], position[1]})
        )
            return false;
        return true;
    }

    public bool IsPositionInsideGrid(int[] position)
    {
        if (position[0] < rows
            && position[1] < columns
            && position[0] >= 0
            && position[1] >= 0)
        {
            return true;
        }
        return false;
    }

    private int[] GetPositionDesiredAndRotate()
    {
        // Check input 
        int[] positionDesired = {-5, -5};
        if (Input.GetKeyDown("up"))
        {
            astronautController.RotatePlayer(Vector3.forward);
            positionDesired = new[] {astronautController.GetX(), astronautController.GetY() + 1};
        }

        else if (Input.GetKeyDown("down"))
        {
            positionDesired = new[] {astronautController.GetX(), astronautController.GetY() - 1};
            astronautController.RotatePlayer(Vector3.back);
        }

        else if (Input.GetKeyDown("right"))
        {
            positionDesired = new[] {astronautController.GetX() + 1, astronautController.GetY()};
            astronautController.RotatePlayer(Vector3.right);
        }

        else if (Input.GetKeyDown("left"))
        {
            positionDesired = new[] {astronautController.GetX() - 1, astronautController.GetY()};
            astronautController.RotatePlayer(Vector3.left);
        }

        return positionDesired;
    }


    public void CreateHeart()
    {
        if (Random.Range(1, 4) == 1)
        {
            int r = Random.Range(1, desertArrIntHashSet.Count);
            if (r < desertArrIntHashSet.Count)
            {
                heartPosition = desertArrIntHashSet.ElementAt(r);
                heartLife.transform.gameObject.SetActive(true);
                heartLife.transform.position = new Vector3(heartPosition[0], 0.5f, heartPosition[1]);
            }
        }
        else
        {
            heartLife.transform.gameObject.SetActive(false);
            heartPosition = new[] {-1, -1};
        }
    }

    private bool DidPlayerFinishedWithDesiredPosition(int[] positionDesired)
    {
        if (_endCondition() && positionDesired[0] == _finalPosition[0] &&
            positionDesired[1] == _finalPosition[1])
            return true;
        return false;
    }


    public void MoveAstronaut()
    {
        int[] positionDesired = GetPositionDesiredAndRotate();

        // Check if position is a position inside the grid
        if (IsPositionAvailable(positionDesired) &&
            !poisonArrIntHashSet.Contains(positionDesired) &&
            !_forestArrIntHashSet.Contains(positionDesired) &&
            !(positionDesired[0] == _finalPosition[0] && positionDesired[1] == _finalPosition[1])
            || DidPlayerFinishedWithDesiredPosition(positionDesired)
        )
        {
            GameObject parent = gridCell[positionDesired[0], positionDesired[1]];

            astronautController.SetAllPositionAstronaut(positionDesired[0], positionDesired[1],
                parent.GetComponent<Cell>().GetPosition());
            TransformIntoForest(positionDesired[0], positionDesired[1]);
            desertArrIntHashSet.Remove(positionDesired);
            CheckIfMovementWithHeart(positionDesired);
        }

        // User failed
        else if (IsPositionAvailable(positionDesired) && positionDesired[0] != -5 && positionDesired[1] != -5 &&
                 (poisonArrIntHashSet.Contains(positionDesired) || _forestArrIntHashSet.Contains(positionDesired)) ||
                 positionDesired[0] == _finalPosition[0] && positionDesired[1] == _finalPosition[1])
        {
            Cell cell = gridCell[startingPosition[0], startingPosition[1]].GetComponent<Cell>();
            astronautController.SetAllPositionAstronaut(startingPosition[0], startingPosition[1], cell.GetPosition());
            ConvertAllForestIntoDesert();
            soundManager.PlayLevelFailed();
            TransformIntoForest(startingPosition[0], startingPosition[1]);
            desertArrIntHashSet.Remove(startingPosition);
            gameController.ReduceScore();
            gameController.DecreaseLive();
        }
    }

    void CheckIfMovementWithHeart(int[] positionDesired)
    {
        if (positionDesired[0] == heartPosition[0] && positionDesired[1] == heartPosition[1])
        {
            heartLife.transform.gameObject.SetActive(false);
            heartPosition = new[] {-1, -1};
            gameController.IncreaseLive();
            soundManager.PlayHeartPicked();
        }
    }


    private void TransformTerrainIntoAnother(int x, int y, CellType cellType)
    {
        GameObject parent = gridCell[x, y];
        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }

        GameObject go = GetRandomFromList(_dictCellTypeListGameObjects[cellType]);

        GameObject cellGameObject = Instantiate(go, parent.transform.position, Quaternion.identity, parent.transform);
        cellGameObject.transform.parent = parent.transform;
    }

    private void TransformIntoForest(int x, int y)
    {
        TransformTerrainIntoAnother(x, y, CellType.Forest);
        _forestArrIntHashSet.Add(new[] {x, y});
    }

    public void TransformIntoPoison(int x, int y)
    {
        TransformTerrainIntoAnother(x, y, CellType.DesertPoison);
        poisonArrIntHashSet.Add(new[] {x, y});
    }

    private void TransformIntoFinalCell(int x, int y)
    {
        TransformTerrainIntoAnother(x, y, CellType.Final);
    }

    public void TransformIntoDesert(int x, int y)
    {
        TransformTerrainIntoAnother(x, y, CellType.Desert);
        desertArrIntHashSet.Add(new[] {x, y});
    }

    private void ConvertAllForestIntoDesert()
    {
        foreach (var forest in _forestArrIntHashSet)
            TransformIntoDesert(forest[0], forest[1]);
        _forestArrIntHashSet.Clear();
    }


    public bool IsLevelFinished()
    {
        int[] pos = {astronautController.GetX(), astronautController.GetY()};
        return DidPlayerFinishedWithDesiredPosition(pos);
    }

    public void DestroyLevel()
    {
        gridCell = new GameObject[rows, columns];
        DestroyAllChildren();
        desertArrIntHashSet.Clear();
        poisonArrIntHashSet.Clear();
        _forestArrIntHashSet.Clear();
        emptyArrIntsHashSet.Clear();
    }

    public void DestroyAllChildren()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}