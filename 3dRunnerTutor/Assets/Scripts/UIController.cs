using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    //Cтартовая панель
    [SerializeField] private GameObject startPanel;
    [SerializeField] private Text bestScoreText;
    [SerializeField] private Text countCoinsText;

    //Игровая панель
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private Text coinsForSessionText;
    [SerializeField] private Text scoreForSessionText;

    //Панель паузы
    [SerializeField] private GameObject pausePanel;

    //Панель ГеймОвер
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text scoreForSession;
    [SerializeField] private Text countOfCoinForSession;
    //[SerializeField] private GameObject bestScoreSprite;
    
    private int _coinsForSession;
    private int _scoreForSession;
    private float _bonusScoreForTurn;
    private bool _isTurned = false;
    private float speedPlayer;
    private bool gameIsStarted = false;
    private bool gameIsPaused = false;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("Coin"))
        {
            PlayerPrefs.SetInt("Coin", 0);
        }
        if (!PlayerPrefs.HasKey("BestScore"))
        {
            PlayerPrefs.SetInt("BestScore", 0);
        }
        GlobalEventManager.OnMoneyAdd.AddListener(AddCoin);
        GlobalEventManager.OnChunkComplete.AddListener(AddScore);
        GlobalEventManager.OnGameOver.AddListener(GameOver);
        
    }

    void Start()
     {
         SetStartMenu();
         coinsForSessionText.text = _coinsForSession.ToString();
         scoreForSessionText.text = _scoreForSession.ToString();
     }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void SetStartMenu()
    {
        gamePanel.SetActive(false);
        pausePanel.SetActive(false);

        startPanel.SetActive(true);
        countCoinsText.text = PlayerPrefs.GetInt("Coins").ToString();
        bestScoreText.text = PlayerPrefs.GetInt("BestScore").ToString();
    }

    public void StartGame()
    {
        gameIsStarted = true;
        
        startPanel.SetActive(false);
        gamePanel.SetActive(true);

        _coinsForSession = 0;
        coinsForSessionText.text = _coinsForSession.ToString();

        _scoreForSession = 0;
        scoreForSessionText.text = _scoreForSession.ToString();

        _bonusScoreForTurn = 0;
        
        GlobalEventManager.OnGameStart.Invoke();
    }

    public void StartPause()
    {
        GlobalEventManager.OnStartPause.Invoke();
        gameIsPaused = true;
        
        gamePanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void FinishPause()
    {
        pausePanel.SetActive(false);
        gamePanel.SetActive(true);
        
        gameIsPaused = false;
        
        GlobalEventManager.OnFinishPause.Invoke();
    }

    private void GameOver()
    {
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);

        scoreForSession.text = _scoreForSession.ToString();

        SaveData(_coinsForSession, _scoreForSession);
    }

    private void SaveData(int numberOfCoins, int score)
    {
        if (PlayerPrefs.GetInt("BestScore") < score)
        {
            PlayerPrefs.SetInt("BestScore", score);
        }
        PlayerPrefs.SetInt("Coins",PlayerPrefs.GetInt("Coins") + numberOfCoins);
    }
    

    private void AddScore()
    {
        _scoreForSession++;
        scoreForSessionText.text = _scoreForSession.ToString();
    }

    private void AddCoin()
    {
        _coinsForSession++;
        coinsForSessionText.text = _coinsForSession.ToString();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gameIsStarted)
        {
            if(!gameIsPaused)
                StartPause();
            else 
                FinishPause();
        }

        if (Input.GetKeyDown(KeyCode.Space) && !gameIsStarted)
        {
            StartGame();
        }
    }
}
