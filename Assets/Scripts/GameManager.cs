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
    //
    private int maxLives = 3;
    private int currentLives;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        currentLives = maxLives;
        Application.targetFrameRate = 75;
    }

    private void Update()
    {
        UpdateTimer();
    }

    
    
    private void UpdateTimer()
    {
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void LoseOneLife()
    {
        if (currentLives >0)
        {
            currentLives--;
            return;
        }
        Debug.Log("game over");
        
    }
    public void NewGame()
    {
        SudokuBoard.Instance.GenerateSudoku(Random.Range(25,40));
    }
}
