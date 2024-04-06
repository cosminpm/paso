using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.LevelTypes;
using TMPro;
using Random = UnityEngine.Random;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private LongestPath _longestPath;
    private Grid _grid;
    private AllCellLevel _allCellLevel;
    private MaximizeCellLevel _maximizeCellLevel;

    public bool drawGizmos;
    private FollowPlayerCamera _cameraController;

    private SoundManager _soundManager;
    private int _numberOfLevels = 1;
    private TextMeshProUGUI _numberOfLevelsTextMeshProIn;
    private TextMeshProUGUI _numberOfLevelsTextMeshProOut;
    private TextMeshProUGUI _timerText;
    private TextMeshProUGUI _scoreText;

    public bool playing = true;
    private static float _currentTimer;
    private static int _hours, _minutes, _seconds;
    private static int _score;
    private static float _startLevelTimer;

    private List<LevelCell> _typeLevels;
    private int _userLives = 3;

    private void Start()
    {
        InitializeVariables();
        CreateLevel(_allCellLevel);
    }

    private void Update()
    {
        CreateNewLevel();
        _grid.MoveAstronaut();
        UpdateTimer();
    }


    private void UpdateTimer()
    {
        if (playing)
        {
            _currentTimer += Time.deltaTime;
            _hours = Mathf.FloorToInt(_currentTimer / 3600F);
            _minutes = Mathf.FloorToInt(_currentTimer / 60F);
            _seconds = Mathf.FloorToInt(_currentTimer % 60F);
            _timerText.text = TimeToText();
        }
    }

    private string TimeToText()
    {
        return _hours.ToString("00") + ":" + _minutes.ToString("00") + ":" + _seconds.ToString("00");
    }

    private void _updateScore()
    {
        if (_numberOfLevels == 1)
        {
            _score = 0;
        }
        else
        {
            float divider = _currentTimer - _startLevelTimer;
            if (divider < 1)
            {
                divider = 1;
            }

            _score += Convert.ToInt32(_numberOfLevels * 50 / divider);
        }

        _scoreText.text = _score.ToString();
    }

    private void UpdateUI()
    {
        _numberOfLevels += 1;
        _numberOfLevelsTextMeshProIn.text = _numberOfLevels.ToString();
        _numberOfLevelsTextMeshProOut.text = _numberOfLevels.ToString();
    }

    private void InitializeVariables()
    {
        
        _soundManager = GetComponent<SoundManager>();
        _grid = GameObject.Find("Grid").GetComponent<Grid>();
        _longestPath = GameObject.Find("Grid").GetComponent<LongestPath>();
        _cameraController = GameObject.Find("Main Camera").GetComponent<FollowPlayerCamera>();

        _allCellLevel = new AllCellLevel(_grid, _longestPath);
        _maximizeCellLevel = new MaximizeCellLevel(_grid);

        _typeLevels = new List<LevelCell> {_allCellLevel, _maximizeCellLevel};
        
        // First level will be always a allCellLevel
        _grid.SetEndCondition(_allCellLevel.EndCondition);
        
        _grid.astronautController = GameObject.Find("Astronaut").GetComponent<AstronautController>();
        _grid.soundManager = _soundManager;
        _grid.InstantiateDictionaryCellType();
        _soundManager = GetComponent<SoundManager>();

        _numberOfLevelsTextMeshProIn = GameObject.Find("Canvas").transform.Find("NumberOfLevelsIn")
            .GetComponent<TextMeshProUGUI>();
        _numberOfLevelsTextMeshProOut = GameObject.Find("Canvas").transform.Find("NumberOfLevelsOut")
            .GetComponent<TextMeshProUGUI>();
        _timerText = GameObject.Find("Canvas").transform.Find("TimerText").GetComponent<TextMeshProUGUI>();
        _scoreText = GameObject.Find("Canvas").transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        _grid.gameController = this;
        _grid.heartLife = GameObject.Find("HeartLife");
    }


    private void CreateNewLevel()
    {
        if (_grid.IsLevelFinished())
        {
            _soundManager.PlayLevelCompleted();
            UpdateUI();
            _grid.DestroyLevel();
            CreateLevel(GetRandomLevelType());
        }
    }

    private void CreateLevel(LevelCell typeLevel)
    {
        typeLevel.CreateLevel();
        Debug.Log(typeLevel);
        _grid.SetEndCondition(typeLevel.EndCondition);

        _updateScore();
        _cameraController.SetCameraMiddleMap(_grid.rows, _grid.columns, _grid.sizeOfCell.x);
        _startLevelTimer = _currentTimer;
    }

    private LevelCell GetRandomLevelType()
    {
        int randomIndex = Random.Range(0, _typeLevels.Count);
        return _typeLevels[randomIndex];
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
        if (_userLives == 3)
        {
            GameObject.Find("Canvas").transform.Find("hearts").transform.Find("h3").transform.gameObject
                .SetActive(false);
        }
        else if (_userLives == 2)
        {
            GameObject.Find("Canvas").transform.Find("hearts").transform.Find("h2").transform.gameObject
                .SetActive(false);
        }
        else if (_userLives == 1)
        {
            GameObject.Find("Canvas").transform.Find("hearts").transform.Find("h1").transform.gameObject
                .SetActive(false);
        }
        else
        {
            SceneManager.LoadScene("You Died");
            SaveDataBetweenLevels();
        }

        _userLives -= 1;
    }

    private void SaveDataBetweenLevels()
    {
        PlayerPrefs.SetInt("Score", _score);
        PlayerPrefs.SetString("Time", TimeToText());
        PlayerPrefs.SetInt("Level", _numberOfLevels);
    }


    public void IncreaseLive()
    {
        if (_userLives == 2)
        {
            GameObject.Find("Canvas").transform.Find("hearts").transform.Find("h3").transform.gameObject
                .SetActive(true);
            _userLives += 1;
        }
        else if (_userLives == 1)
        {
            GameObject.Find("Canvas").transform.Find("hearts").transform.Find("h2").transform.gameObject
                .SetActive(true);
            _userLives += 1;
        }
        else if (_userLives == 0)
        {
            GameObject.Find("Canvas").transform.Find("hearts").transform.Find("h1").transform.gameObject
                .SetActive(true);
            _userLives += 1;
        }
        else
        {
            _score += 50;
            _scoreText.text = _score.ToString();
        }
    }
}