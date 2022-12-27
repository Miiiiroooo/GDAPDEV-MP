using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    // singleton
    public static ScoreManager Instance { get; private set; }

    // Fields
    [SerializeField] private float highScore = -1;
    [SerializeField] private float currentScore = -1;
    [SerializeField] private bool isMainMenuCurrentScene = false;
    [SerializeField] private bool isTakingScore = false;


    // Properties
    public float CurrentScore
    {
        get { return this.currentScore; }
        private set { this.currentScore = value; }
    }


    // Unity Methods
    void Awake()
    {
        // singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;

            GameObject.DontDestroyOnLoad(this.gameObject);
            SceneManager.activeSceneChanged += OnChangeScene;
            EventBroadcaster.Instance.AddObserver(EventNames.ON_CHANGE_GAME_STATE, OnChangeGameState);
        }
    }

    void OnDestroy()
    {
        if (Instance != null && Instance == this)
        {
            SceneManager.activeSceneChanged -= OnChangeScene;
            EventBroadcaster.Instance.RemoveObserver(EventNames.ON_CHANGE_GAME_STATE);
        }
    }

    void Update()
    {
        if (!this.isMainMenuCurrentScene && this.isTakingScore)
            this.currentScore += Time.deltaTime;
    }

    // Methods for Delegates
    private void OnChangeGameState()
    {
        GameManager.GameStates currentState = GameManager.Instance.GetCurrentGameState();

        switch (currentState)
        {
            case GameManager.GameStates.Countdown:
                StopTakingScore();
                break;
            case GameManager.GameStates.Gameplay:
                ResumeTakingScore();
                break;
            case GameManager.GameStates.GameOver:
                StopTakingScore();
                break;
            case GameManager.GameStates.BossDefeated:
                StopTakingScore();
                break;
            case GameManager.GameStates.Shop:
                StopTakingScore();
                break;
            case GameManager.GameStates.Settings:
                StopTakingScore();
                break;
            case GameManager.GameStates.Debug:
                StopTakingScore();
                break;
        }
    }

    private void OnChangeScene(Scene previousScene, Scene currentScene)
    {
        this.isMainMenuCurrentScene = (currentScene.name == SceneNames.MAIN_MENU_SCENE);

        if (currentScene.name == SceneNames.MAIN_MENU_SCENE || currentScene.name == SceneNames.TUTORIAL_SCENE || currentScene.name == SceneNames.FIRST_LEVEL_SCENE)
            this.currentScore = 0.0f;
    }


    // Other User-Defined Methods
    private void StopTakingScore()
    {
        this.isTakingScore = false;
    }

    private void ResumeTakingScore()
    {
        this.isTakingScore = true;
    }

    public string GetScoreInStringFormat()
    {
        return ConvertFloatScoreIntoStringScore(this.currentScore);
    }

    public string ConvertFloatScoreIntoStringScore(float numScore)
    {
        string stringScore;

        int minutes = (int)Math.Floor(numScore) / 60;
        int seconds = (int)Math.Floor(numScore) - minutes * 60;
        int milliseconds = (int)((decimal)numScore % 1 * 100);

        stringScore = minutes + ":" + seconds + ":" + milliseconds;

        return stringScore;
    }
}
