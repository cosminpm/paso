using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private LongestPath _longestPath;
    private Grid _grid;

    public bool drawGizmos;
    private List<int[]> _longestPathListCells;
    private FollowPlayerCamera _cameraController;
    
    private SoundManager _soundManager;
    private int _numberOfLevels = 1;
    private TextMeshProUGUI _numberOfLevelsTextMeshProIn;
    private TextMeshProUGUI _numberOfLevelsTextMeshProOut;
    private TextMeshProUGUI _timerText;
    private TextMeshProUGUI _scoreText;
    
    public bool playing = true;
    private float _currentTimer;
    private int _hours, _minutes, _seconds; 
    private int _score;
    private float _startLevelTimer;

    private int _userLives = 0;
    
    private void Start()
    {
        InitializeVariables();
        CreateLevel();
    }
    private void Update()
    {
        CreateNewLevel();
        _grid.MoveAstronaut();
        UpdateTimer();
    }


    private void UpdateTimer()
    {
        if(playing){
            _currentTimer += Time.deltaTime;
             _hours = Mathf.FloorToInt(_currentTimer / 3600F);
             _minutes = Mathf.FloorToInt(_currentTimer / 60F);
             _seconds = Mathf.FloorToInt(_currentTimer % 60F);
            _timerText.text = _hours.ToString ("00") + ":" + _minutes.ToString ("00") + ":" + _seconds.ToString ("00");
        }
    }

    private void _updateScore()
    {
        if (_numberOfLevels == 1)
        {
            _score = 0;
        }
        else
        {
            _score +=  Convert.ToInt32(_numberOfLevels * 50 / (_currentTimer - _startLevelTimer));
        }
        _scoreText.text = _score.ToString();

    }

    private void CreateNewLevel()
    {
        if (_grid.IsLevelFinished())
        {
            _grid.SetRandomLimiterPerlinNoise(.5f, .75f);
            _grid.SetRandomScaler(2f, 10f);
            _grid.SetRandomColumnsAndRows(3,7);
            _soundManager.PlayLevelCompleted();
            _numberOfLevels += 1;
            _numberOfLevelsTextMeshProIn.text = _numberOfLevels.ToString();
            _numberOfLevelsTextMeshProOut.text = _numberOfLevels.ToString();

            DestroyLevel();
            CreateLevel();
        }
    }
    
    private void InitializeVariables()
    {
        _soundManager = GetComponent<SoundManager>();
        _grid = GameObject.Find("Grid").GetComponent<Grid>();
        _longestPath = GameObject.Find("Grid").GetComponent<LongestPath>();
        _cameraController = GameObject.Find("Main Camera").GetComponent<FollowPlayerCamera>();

        _grid.astronautController = GameObject.Find("Astronaut").GetComponent<AstronautController>();
        _grid.soundManager = _soundManager;
        _grid.InstantiateDictionaryCellType();
        _soundManager  = GetComponent<SoundManager>();
        
        _numberOfLevelsTextMeshProIn = GameObject.Find("Canvas").transform.Find("NumberOfLevelsIn").GetComponent<TextMeshProUGUI>();
        _numberOfLevelsTextMeshProOut = GameObject.Find("Canvas").transform.Find("NumberOfLevelsOut").GetComponent<TextMeshProUGUI>();
        _timerText = GameObject.Find("Canvas").transform.Find("TimerText").GetComponent<TextMeshProUGUI>();
        _scoreText = GameObject.Find("Canvas").transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        _grid.gameController = this;

    }

    private void CreateLevel()
    {
        _grid.InstantiateGrid();
        _grid.InstantiateAstronaut();
        _updateScore();
        
        SetDFSSize();
        _longestPath.InitializeDFS();

        _longestPathListCells = _longestPath.FindLongestPath(_grid.startingPosition, _grid.poisonArrIntHashSet);


        TransformUnusedDesertIntoPoison(_longestPathListCells);
        
        _grid.CreateFinalCellPosition(_longestPathListCells.Last());
        _cameraController.SetCameraMiddleMap(_grid.rows, _grid.columns, _grid.sizeOfCell.x);
        _startLevelTimer = _currentTimer;

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
        IEnumerable<int[]> difference =
            _grid.desertArrIntHashSet.Except(hashUsedInPath, new IntArrayEqualityComparer());
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

    public void ReduceScore()
    {
        _score -= 50;
        if (_score < 0)
        {
            _score = 0;
        }
        _scoreText.text = _score.ToString();
    }

    public void DecreaseLive()
    {
        _userLives -= 1;
    }
    
}