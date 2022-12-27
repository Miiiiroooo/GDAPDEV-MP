using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DebugHandler : MonoBehaviour
{
    #region singleton and unity methods
    // singleton
    public static DebugHandler Instance { get; private set; }

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
        }
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name != SceneNames.MAIN_MENU_SCENE && SceneManager.GetActiveScene().name != SceneNames.TUTORIAL_SCENE)
        {
            this.playerScript = GameManager.Instance.GetPlayerScript();
        }

        UpdateFromPreviousDebugData();
    }
    #endregion

    #region fields
    // player script and debug other values
    [SerializeField] private PlayerScript playerScript = null;
    [SerializeField] private bool isPlayerInvulvenrable = false;
    [SerializeField] private bool hasUnlimitedBullets = false;

    // UI for the cheats
    [SerializeField] private Toggle invulnerabilityToggle;
    [SerializeField] private Toggle unlimitedBulletsToggle;
    [SerializeField] private TMPro.TMP_InputField addGoldInput;

    // UI for notifications
    [SerializeField] private TMPro.TMP_InputField generalNotifInput;
    [SerializeField] private TMPro.TMP_InputField generalFireTimeInput;
    [SerializeField] private TMPro.TMP_InputField socialNotifInput;
    [SerializeField] private TMPro.TMP_InputField socialFireTimeInput;

    // UI for the leaderboards
    [SerializeField] private Button createGroupButton;
    [SerializeField] private Button createPlayerButton;
    [SerializeField] private TMPro.TMP_InputField nameInput;
    [SerializeField] private TMPro.TMP_InputField scoreInput;
    [SerializeField] private Button getScoresButton;
    [SerializeField] private Button resetScoresButton;
    [SerializeField] private TMPro.TMP_InputField secretInput;
    #endregion


    // properties
    public bool IsPlayerInvulvenrable
    {
        get { return this.isPlayerInvulvenrable; }
        set { this.isPlayerInvulvenrable = value; }
    }

    public bool HasUnlimitedBullets
    {
        get { return this.hasUnlimitedBullets;  }
        set { this.hasUnlimitedBullets = value; }
    }


    // Methods for General Debugging
    public void UpdateDebugDataToGameManager()
    {
        DebugData newData = new()
        {
            IsPlayerInvulnerable = this.IsPlayerInvulvenrable,
            HasUnlimitedBullets = this.hasUnlimitedBullets,
        };

        GameManager.Instance.UpdateDebugData(newData);
    }

    private void UpdateFromPreviousDebugData()
    {
        DebugData prevData = GameManager.Instance.GetDebugData();

        this.isPlayerInvulvenrable = prevData.IsPlayerInvulnerable;
        this.hasUnlimitedBullets = prevData.HasUnlimitedBullets;

        this.invulnerabilityToggle.isOn = this.isPlayerInvulvenrable;
        this.unlimitedBulletsToggle.isOn = this.hasUnlimitedBullets;
    }


    // Methods for the Cheats
    public void AddGold()
    {
        if (this.playerScript != null)
        {
            int amount = Int32.Parse(this.addGoldInput.text);
            this.playerScript.AddPlayerGold(amount);
        }
        else 
        {
            string displayText = "Invalid Debug.\nPlease play the game to add gold to a player.";
            OutputDisplayHandler.Instance.DisplayText(displayText);
        }
    }

    public void KillPlayer()
    {
        if (SceneManager.GetActiveScene().name != SceneNames.MAIN_MENU_SCENE && SceneManager.GetActiveScene().name != SceneNames.TUTORIAL_SCENE)
        {
            GameManager.Instance.UpdateGameState(GameManager.GameStates.GameOver);
        }
        else
        {
            string displayText = "Invalid Debug.\nPlease play the game to kill the player.";
            OutputDisplayHandler.Instance.DisplayText(displayText);
        }

    }

    public void KillBoss()
    {
        if (SceneManager.GetActiveScene().name != SceneNames.MAIN_MENU_SCENE && SceneManager.GetActiveScene().name != SceneNames.TUTORIAL_SCENE)
        {
            GameManager.Instance.UpdateGameState(GameManager.GameStates.BossDefeated);
        }
        else
        {
            string displayText = "Invalid Debug.\nPlease play the game to kill the boss.";
            OutputDisplayHandler.Instance.DisplayText(displayText);
        }
    }


    // Methods for Levels
    public void OnGoToTutorial()
    {
        SceneLoader.Instance.LoadLevel(SceneNames.TUTORIAL_SCENE);
    }

    public void OnGoToFirstLevel()
    {
        SceneLoader.Instance.LoadLevel(SceneNames.FIRST_LEVEL_SCENE);
    }

    public void OnGoToSecondLevel()
    {
        SceneLoader.Instance.LoadLevel(SceneNames.SECOND_LEVEL_SCENE);
    }

    public void OnGoToThirdLevel()
    {
        SceneLoader.Instance.LoadLevel(SceneNames.THIRD_LEVEL_SCENE);
    }

    public void OnGoToMainMenu()
    {
        SceneLoader.Instance.LoadLevel(SceneNames.MAIN_MENU_SCENE);
    }


    // Methods for Notifcations
    public void PostToGeneralNotification()
    {
        if (this.generalNotifInput.text == "")
            return;

        string debug_message = this.generalNotifInput.text;

        if (this.generalFireTimeInput.text == "")
        {
            NotificationHandler.Instance.PostGeneralNotifications(debug_message);
        }
        else
        {
            int fireTime = Int32.Parse(this.generalFireTimeInput.text);
            NotificationHandler.Instance.PostGeneralNotifications(debug_message, fireTime);
        }
    }

    public void PostToSocialNotification()
    {
        if (this.socialNotifInput.text == "")
            return;

        string debug_message = this.socialNotifInput.text;

        if (this.socialFireTimeInput.text == "")
        {
            NotificationHandler.Instance.PostSocialNotifications(message: debug_message);
        }
        else
        {
            int fireTime = Int32.Parse(this.socialFireTimeInput.text);
            NotificationHandler.Instance.PostSocialNotifications(message: debug_message, fireTimeInput: fireTime);
        }
    }


    // Methods for Ads


    // Methods for the Leaderboards
    public void CreateGroupToServer()
    {
        WebHandler.Instance.CreateGroupToServer(this.createGroupButton);
    }

    public void CreateNewPlayerHighScore()
    {
        if (this.nameInput.text == "" || this.scoreInput.text == "")
            return;

        string userName = this.nameInput.text;
        float score = float.Parse(this.scoreInput.text);

        WebHandler.Instance.CreateNewPlayerHighScore(userName, score, true, this.createPlayerButton);
    }

    public void GetScores()
    {
        WebHandler.Instance.GetLeaderboardScores(this.getScoresButton);
    }

    public void ResetLeaderboard()
    {
        if (this.secretInput.text == "")
            return;

        string secret = this.secretInput.text;
        WebHandler.Instance.ResetLeaderboard(secret, this.resetScoresButton);
    }
}
