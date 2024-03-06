using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private LongestPath _longestPath;
    private Grid _grid;

    public bool drawGizmos;
    private List<int[]> _longestPathListCells;
    private FollowPlayerCamera _cameraController;
    private Astronaut _astronaut;
    private SoundManager _soundManager;
    
    
    private void Start()
    {
        InitializeVariables();
        CreateLevel();
    }
    private void Update()
    {
        CreateNewLevel();
        _grid.MoveAstronaut();
    }

    private void CreateNewLevel()
    {
        if (_grid.IsLevelFinished())
        {
            _grid.SetRandomLimiterPerlinNoise(.5f, .75f);
            _grid.SetRandomScaler(2f, 10f);
            _grid.SetRandomColumnsAndRows(3,7);
            _soundManager.PlayLevelCompleted();
            DestroyLevel();
            CreateLevel();
        }
    }
    
    private void InitializeVariables()
    {
        _grid = GameObject.Find("Grid").GetComponent<Grid>();
        _longestPath = GameObject.Find("Grid").GetComponent<LongestPath>();
        _cameraController = GameObject.Find("Main Camera").GetComponent<FollowPlayerCamera>();
        _grid.InstantiateDictionaryCellType();
        _soundManager  = GetComponent<SoundManager>();
    }

    private void CreateLevel()
    {
        _grid.InstantiateGrid();
        
        _grid.InstantiateAstronaut();

        SetDFSSize();
        _longestPath.InitializeDFS();

        _longestPathListCells = _longestPath.FindLongestPath(_grid.startingPosition, _grid.poisonArrIntHashSet);


        TransformUnusedDesertIntoPoison(_longestPathListCells);
        _grid.CreateFinalCellPosition(_longestPathListCells.Last());
        _astronaut = GameObject.Find("Astronaut").GetComponent<Astronaut>();
        _cameraController.SetCameraMiddleMap(_grid.rows, _grid.columns, _grid.sizeOfCell.x);
    }

    private void DestroyLevel()
    {
        _grid.DestroyLevel();
    }


    private void SetDFSSize()
    {
        _longestPath.columns = _grid.columns;
        _longestPath.rows = _grid.rows;
    }

    private void TransformUnusedDesertIntoPoison(List<int[]> usedInPath)
    {
        HashSet<int[]> hashUsedInPath = new HashSet<int[]>(new IntArrayEqualityComparer());
        hashUsedInPath.AddRange(usedInPath);

        // Find the difference between _grid.desertArrIntHashSet and hashUsedInPath
        IEnumerable<int[]> difference =
            _grid.desertArrIntHashSet.Except(hashUsedInPath, new IntArrayEqualityComparer());

        // Convert the result to a list
        List<int[]> result = difference.ToList();

        foreach (var cell in result)
        {
            _grid.TransformIntoPoison(cell[0], cell[1]);
            _grid.desertArrIntHashSet.Remove(cell);
        }
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            _longestPath.DrawDebugVisited(_longestPathListCells, _grid.gridCell);
        }
    }
}