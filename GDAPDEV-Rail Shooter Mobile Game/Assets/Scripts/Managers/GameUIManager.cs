using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    #region FIELDS/ATRIBUTES
    // singleton
    public static GameUIManager Instance { get; private set; }

    // UI windows
    [SerializeField] private GameObject gameplayHUD;
    [SerializeField] private GameObject bossDefeatedWindow;
    [SerializeField] private GameObject shopWindow;
    [SerializeField] private GameObject gameOverWindow;
    [SerializeField] private GameObject settingsWindow;
    [SerializeField] private GameObject debugWindow;

    // UI texts
    [SerializeField] private TMPro.TextMeshProUGUI HP_Text;
    [SerializeField] private TMPro.TextMeshProUGUI ammo_Text;
    [SerializeField] private TMPro.TextMeshProUGUI timeScore_Text;
    [SerializeField] private TMPro.TextMeshProUGUI gold_Text;
    [SerializeField] private TMPro.TextMeshProUGUI gold_Shop_Text;

    // UI images
    [SerializeField] private Image redWeaponUI;
    [SerializeField] private Image greenWeaponUI;
    [SerializeField] private Image blueWeaponUI;

    // UI only when player wins a level
    [SerializeField] private GameObject bossDefeatedMenu;
    [SerializeField] private GameObject winGameMenu;
    [SerializeField] private GameObject nameInputMenu;
    [SerializeField] private GameObject playerResultsMenu;
    [SerializeField] private TMPro.TMP_InputField nameInputField;
    [SerializeField] private Button leaderboardsButton;
    [SerializeField] private TMPro.TextMeshProUGUI playerName_Text;
    [SerializeField] private TMPro.TextMeshProUGUI playerScore_Text;
    [SerializeField] private Button returnToMainbutton;

    // additional fields
    private PlayerScript playerScript;
    #endregion


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
            EventBroadcaster.Instance.AddObserver(EventNames.ON_CHANGE_GAME_STATE, OnChangeGameState);
        }
    }

    void OnDestroy()
    {
        if (Instance != null && Instance == this)
        {
            EventBroadcaster.Instance.RemoveActionAtObserver(EventNames.ON_CHANGE_GAME_STATE, OnChangeGameState);
        }
    }

    void Start()
    {
        this.playerScript = GameManager.Instance.GetPlayerScript();

        UpdateHUD();
        UpdateWeaponColorUI();

        DisableAllWindows();
        this.gameplayHUD.SetActive(true);
    }

    void Update()
    {
        if (this.gameplayHUD.activeSelf)
            UpdateHUD();
        else if (this.shopWindow.activeSelf)
            this.gold_Shop_Text.text = playerScript.CurrentGold.ToString();
    }


    // Methods for Delegates
    private void OnChangeGameState()
    {
        GameManager.GameStates currentState = GameManager.Instance.GetCurrentGameState();

        switch (currentState)
        {
            case GameManager.GameStates.Countdown:
                AdsManager.Instance.GetBannerAd().HideBannerAd();
                break;

            case GameManager.GameStates.Gameplay:
                DisableAllWindows();
                this.gameplayHUD.SetActive(true);
                Time.timeScale = 1.0f;
                AdsManager.Instance.GetBannerAd().ShowBannerAd(false);
                break;

            case GameManager.GameStates.GameOver:
                DisableAllWindows();
                this.gameOverWindow.SetActive(true);
                Time.timeScale = 0.0f;
                AdsManager.Instance.GetBannerAd().HideBannerAd();
                break;

            case GameManager.GameStates.BossDefeated:
                DisableAllWindows();
                this.bossDefeatedWindow.SetActive(true);
                CheckForEndGameCondition();
                Time.timeScale = 0.0f;
                AdsManager.Instance.GetBannerAd().HideBannerAd();
                break;

            case GameManager.GameStates.Shop:
                DisableAllWindows();
                this.shopWindow.SetActive(true);
                AdsManager.Instance.GetBannerAd().HideBannerAd();
                break;

            case GameManager.GameStates.Settings:
                DisableAllWindows();
                this.settingsWindow.SetActive(true);
                Time.timeScale = 0.0f;
                AdsManager.Instance.GetBannerAd().HideBannerAd();
                break;

            case GameManager.GameStates.Debug:
                DisableAllWindows();
                this.debugWindow.SetActive(true);
                AdsManager.Instance.GetBannerAd().HideBannerAd();
                break;
        }
    }


    // Methods for UI Elements
    public void OnClickSettings()
    {
        GameManager.Instance.UpdateGameState(GameManager.GameStates.Settings);
    }

    public void OnResume()
    {
        GameManager.Instance.UpdateGameState(GameManager.GameStates.Gameplay);
    }

    public void OnDebugButton()
    {
        GameManager.Instance.UpdateGameState(GameManager.GameStates.Debug);
    }

    public void OnCloseDebugButton()
    {
        GameManager.Instance.UpdateGameState(GameManager.GameStates.Settings);
    }

    public void OnWatchRewardedAd()
    {
        AdsManager.Instance.GetRewardedAd().ShowAd(false);
    }

    public void OnContinueToShop()
    {
        AdsManager.Instance.GetInterstitialAd().ShowAd();
        GameManager.Instance.UpdateGameState(GameManager.GameStates.Shop);
    }

    public void OnIncreasePlayerHP()
    {
        if (playerScript.DoesPlayerHaveEnoughGold(1000))
        {
            playerScript.IncreaseMaxHP(1);
            playerScript.ReducePlayerGold(1000);
        }
        else
        {
            string warning = "You don't have\nenough gold.";
            OutputDisplayHandler.Instance.DisplayText(warning);
        }
    }

    public void OnIncreasePlayerAmmo()
    {
        if (playerScript.DoesPlayerHaveEnoughGold(200))
        {
            playerScript.IncreaseAmmoCount(1);
            playerScript.ReducePlayerGold(200);
        }
        else
        {
            string warning = "You don't have\nenough gold.";
            OutputDisplayHandler.Instance.DisplayText(warning);
        }
    }

    public void OnContinueToNextLevel()
    {
        if (SceneManager.GetActiveScene().name == SceneNames.TUTORIAL_SCENE)
        {
            SceneLoader.Instance.LoadLevel(SceneNames.FIRST_LEVEL_SCENE);
        }
        else if (SceneManager.GetActiveScene().name == SceneNames.FIRST_LEVEL_SCENE)
        {
            SceneLoader.Instance.LoadLevel(SceneNames.SECOND_LEVEL_SCENE);
        }
        else if (SceneManager.GetActiveScene().name == SceneNames.SECOND_LEVEL_SCENE)
        {
            SceneLoader.Instance.LoadLevel(SceneNames.THIRD_LEVEL_SCENE);
        }
    }

    public void OnContinueToNameInput()
    {
        this.bossDefeatedMenu.SetActive(false);
        this.winGameMenu.SetActive(false);
        this.nameInputMenu.SetActive(true);
        this.playerResultsMenu.SetActive(false);
    }

    public void OnContinueToLeaderboards()
    {
        string userName = this.nameInputField.text;
        float score = ScoreManager.Instance.CurrentScore;

        NotificationHandler.Instance.PostSocialNotifications(score: score);
        WebHandler.Instance.CreateNewPlayerHighScore(userName, score, false, this.returnToMainbutton);
        WebHandler.Instance.GetLeaderboardScores(this.leaderboardsButton);

        this.nameInputMenu.SetActive(false);
        this.playerResultsMenu.SetActive(true);
        this.playerName_Text.text = this.nameInputField.text;
        this.playerScore_Text.text = ScoreManager.Instance.GetScoreInStringFormat();
    }

    public void ReturnToMainMenuScene()
    {
        NotificationHandler.Instance.PostGeneralNotifications();
        SceneLoader.Instance.LoadLevel(SceneNames.MAIN_MENU_SCENE);
    }


    // Other User-Defined Methods
    private void UpdateHUD()
    {
        string score = ScoreManager.Instance.GetScoreInStringFormat();

        HP_Text.text = playerScript.CurrentHP.ToString() + "/" + playerScript.MaxHP.ToString();
        ammo_Text.text = playerScript.GetCurrentWeapon().GetCurrentAmmoCount().ToString() + "/" + playerScript.GetCurrentWeapon().GetMaximumAmmo().ToString();
        timeScore_Text.text = score;
        gold_Text.text = "$" + playerScript.CurrentGold.ToString();
    }

    public void UpdateWeaponColorUI()
    {
        if (playerScript.CurrentWeaponColor.ToString() == "Red")
        {
            this.redWeaponUI.color = new Color(1, 0, 0, 1);
            this.greenWeaponUI.color = new Color(0, 1, 0, 0.4f);
            this.blueWeaponUI.color = new Color(0, 0, 1, 0.4f);
        }
        else if (playerScript.CurrentWeaponColor.ToString() == "Green")
        {
            this.redWeaponUI.color = new Color(1, 0, 0, 0.4f);
            this.greenWeaponUI.color = new Color(0, 1, 0, 1);
            this.blueWeaponUI.color = new Color(0, 0, 1, 0.4f);
        }
        else if (playerScript.CurrentWeaponColor.ToString() == "Blue")
        {
            this.redWeaponUI.color = new Color(1, 0, 0, 0.4f);
            this.greenWeaponUI.color = new Color(0, 1, 0, 0.4f);
            this.blueWeaponUI.color = new Color(0, 0, 1, 1);
        }
    }

    private void DisableAllWindows()
    {
        this.gameplayHUD.SetActive(false);
        this.bossDefeatedWindow.SetActive(false);
        this.shopWindow.SetActive(false);
        this.gameOverWindow.SetActive(false);
        this.settingsWindow.SetActive(false);
        this.debugWindow.SetActive(false);
    }

    private void CheckForEndGameCondition()
    {
        if (SceneManager.GetActiveScene().name == SceneNames.THIRD_LEVEL_SCENE)
        {
            this.bossDefeatedMenu.SetActive(false);
            this.winGameMenu.SetActive(true);
            this.nameInputMenu.SetActive(false);
            this.playerResultsMenu.SetActive(false);
        }
        else
        {
            this.bossDefeatedMenu.SetActive(true);
            this.winGameMenu.SetActive(false);
            this.nameInputMenu.SetActive(false);
            this.playerResultsMenu.SetActive(false);
        }
    }
}
