using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    //timer
    public TextMeshProUGUI timerText;
    private float elapsedTime;
    //pause / resume
    public bool isPaused;
    public GameObject ResumeButton;
    //mistakes
    private int maxMistakes = 3;
    private int mistakesCount;
    public TextMeshProUGUI mistakesCounterText;
    //Result window
    public GameObject ResultWindow;
    public TextMeshProUGUI ResultTimeText;
    public TextMeshProUGUI ResultText;
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        mistakesCount = 0;
        Application.targetFrameRate = 75;
    }

    private void Update()
    {
        UpdateTimer();
    }

    
    
    private void UpdateTimer()
    {
        if (isPaused)
        {
            return;
        }
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    
    
    
    public void IncrementMistakeCount()
    {
        mistakesCount++;
        mistakesCounterText.text = "Mistakes:" + mistakesCount.ToString() +"/3";
        if (mistakesCount > maxMistakes - 1)
        {
            ShowGameResult(false);
            return;
        }
    }
    public void NewGame()
    {
        if (GameManager.Instance.isPaused)
        {
            return;
        }
        if (ResultWindow.activeSelf)
        {
            ResultWindow.SetActive(false);
        }

        elapsedTime = 0f;
        mistakesCount = 0;
        mistakesCounterText.text = "Mistakes:0/3";
        SudokuBoard.Instance.GenerateSudoku(Random.Range(25,40));
        
    }

    public void ShowGameResult(bool value)
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        if (value == false)
        {
            ResultText.text = "You lost!";
        }
        else
        {
            ResultText.text = "You won!";
        }
        ResultTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        ResultWindow.SetActive(true);
    }
    public void TogglePause()
    {
        isPaused = !isPaused;
        ResumeButton.SetActive(isPaused);
        SudokuBoard.Instance.ToggleBoardActive(isPaused);
    }

}
