using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinishScene : MonoBehaviour
{
    private TextMeshProUGUI _numberOfLevelsTextMeshProIn;
    private TextMeshProUGUI _numberOfLevelsTextMeshProOut;
    private TextMeshProUGUI _timerText;
    private TextMeshProUGUI _scoreText;

    private void Start()
    {
        LoadScore();
    }

    void LoadScore()
    {
        _timerText = GameObject.Find("Canvas").transform.Find("All").transform.Find("TimerText")
            .GetComponent<TextMeshProUGUI>();
        _scoreText = GameObject.Find("Canvas").transform.Find("All").transform.Find("ScoreText")
            .GetComponent<TextMeshProUGUI>();
        _numberOfLevelsTextMeshProIn = GameObject.Find("Canvas").transform.Find("All").transform
            .Find("NumberOfLevelsIn").GetComponent<TextMeshProUGUI>();
        _numberOfLevelsTextMeshProOut = GameObject.Find("Canvas").transform.Find("All").transform
            .Find("NumberOfLevelsOut").GetComponent<TextMeshProUGUI>();

        _scoreText.text = PlayerPrefs.GetInt("Score").ToString();
        _timerText.text = PlayerPrefs.GetString("Time");
        _numberOfLevelsTextMeshProOut.text = PlayerPrefs.GetInt("Level").ToString();
        _numberOfLevelsTextMeshProIn.text = PlayerPrefs.GetInt("Level").ToString();
    }
}