using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
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

    // Menu
    public GameObject ResultWindow;
    public TextMeshProUGUI ResultTimeText;
    public TextMeshProUGUI ResultText;
    public GameObject LevelSelectMenu;
    public TextMeshProUGUI DifficultyText;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
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
    
    
    private List<int> GetSudokuData(int difficulty)
    {
        
        string json = File.ReadAllText(Application.dataPath + "/SudokuData/Data.json") ;
        Root sudokuDatabase = JsonUtility.FromJson<Root>(json);
        List<List<int>> matrixList = new List<List<int>>();
        int count = 0;
        foreach (var puzzle in sudokuDatabase.Puzzle)
        {
            if (puzzle.difficulty == difficulty)
            {
                count++;
                matrixList.Add(puzzle.matrix);
            }
        }
        Debug.Log(count);
        int randomLevel = Random.Range(0, matrixList.Count);
        Debug.Log(matrixList.Count);
        Debug.Log(randomLevel);
        return matrixList[randomLevel];

    }


    public void NewGame(int val)
    {


        if (LevelSelectMenu.activeSelf)
        {
            LevelSelectMenu.SetActive(false);
        }
        SudokuBoard.Instance.FillSudokuBoard(GetSudokuData(val));
        DifficultyText.text = Enum.GetName(typeof(Difficulty), val);
        elapsedTime = 0f;
        
    }

    public void ShowGameResult(bool isComplete)
    {
        if (!isComplete)
        {
            return;
        }
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
            ResultText.text = "You won!";
        ResultTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        ResultWindow.SetActive(true);
    }

    public void OpenLevelSelectMenu()
    {
        Debug.Log("here");
        if (GameManager.Instance.isPaused)
        {
            return;
        }
        if (ResultWindow.activeSelf)
        {
            ResultWindow.SetActive(false);
        }

        if (!LevelSelectMenu.activeSelf)
        {
            LevelSelectMenu.SetActive(true);
        }
        
    }
    public void TogglePause()
    {
        isPaused = !isPaused;
        ResumeButton.SetActive(isPaused);
        SudokuBoard.Instance.ToggleBoardActive(isPaused);
    }

}
