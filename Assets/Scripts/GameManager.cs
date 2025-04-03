using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    //timer
    public TextMeshProUGUI timerText;
    private float elapsedTime;
    //
    public int maxLives = 3;
    public int currentLives;
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
}
