using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


# region Structs
public struct PlayerData
{
    public int MaxHP;
    public int CurrentGold;
    public int RedAmmoMaximumCount;
    public int GreenAmmoMaximumCount;
    public int BlueAmmoMaximumCount;

    public void InitDefaultValues()
    {
        this.MaxHP = 3;
        this.CurrentGold = 0;
        this.RedAmmoMaximumCount = 5;
        this.GreenAmmoMaximumCount = 5;
        this.BlueAmmoMaximumCount = 5;
    }
}

public struct DebugData
{
    public bool IsPlayerInvulnerable;
    public bool HasUnlimitedBullets;

    public void InitDefaultValues()
    {
        this.IsPlayerInvulnerable = false;
        this.HasUnlimitedBullets = false;
    }
}
# endregion


public class GameManager : MonoBehaviour
{

    #region Singleton and Unity Methods
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;

            SceneManager.activeSceneChanged += OnChangeScene;
            Object.DontDestroyOnLoad(this.gameObject);

            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;

            this.debugData.InitDefaultValues();
        }
    }

    private void OnDestroy()
    {
        if (Instance != null && Instance == this)
        {
            SceneManager.activeSceneChanged -= OnChangeScene;
            Destroy(this.gameObject);
        }
    }
    #endregion

    #region Enums
    public enum GameStates
    {
        Unknown,
        Countdown,
        Gameplay,
        GameOver,
        BossDefeated,
        Shop,
        Settings,
        Debug,
    }

    public enum GamePhase
    {
        Unknown,
        Phase1,
        Phase2,
        BossFight,
    }
    #endregion

    // game management
    public const string GAME_NAME = "Mecha Madness";
    private GameStates currentState = GameStates.Unknown;
    private GamePhase currentPhase = GamePhase.Unknown;
    private PlayerData playerData;
    private DebugData debugData;

    // player
    [SerializeField] private Transform playerTransform;
    [SerializeField] private PlayerScript playerScript;


    // methods for game states and phases
    public GameStates GetCurrentGameState()
    {
        return this.currentState;
    }

    public void UpdateGameState(GameStates newState)
    {
        this.currentState = newState;
        EventBroadcaster.Instance.PostEvent(EventNames.ON_CHANGE_GAME_STATE);
    }

    public GamePhase GetCurrentGamePhase()
    {
        return this.currentPhase;
    }

    public void UpdateGamePhase(GamePhase nextPhase)
    {
        this.currentPhase = nextPhase;
    }

    // methods for the player
    public PlayerData GetPlayerData()
    {
        return this.playerData;
    }

    public void UpdatePlayerData()
    {
        if (this.playerScript == null)
            return;

        PlayerData newData = playerScript.CreatePlayerData();
        this.playerData = newData;
    }

    private void ResetPlayerData()
    {
        this.playerData.InitDefaultValues();
    }

    public Transform GetPlayerTransform()
    {
        return playerTransform;
    }

    public PlayerScript GetPlayerScript()
    {
        return this.playerScript;
    }

    // methods for debugging
    public DebugData GetDebugData()
    {
        return this.debugData;
    }

    public void UpdateDebugData(DebugData newData)
    {
        this.debugData = newData;
    }

    // methods for delegates
    private void OnChangeScene(Scene prevScene, Scene currentScene)
    {
        if (currentScene.name == SceneNames.MAIN_MENU_SCENE)
        {
            ResetPlayerData();
            this.playerTransform = null;
            this.playerScript = null;

            UpdateGameState(GameStates.Unknown);
        }
        else
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            this.playerTransform = player.GetComponent<Transform>();
            this.playerScript = player.GetComponent<PlayerScript>();


            // TEMPORARY remove upon use
            UpdateGameState(GameStates.Gameplay);
        }
    }
}
