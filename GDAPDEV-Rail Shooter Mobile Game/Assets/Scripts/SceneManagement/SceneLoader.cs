using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider loadingScreenSlider;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        this.loadingScreen.SetActive(false);
    }

    public void LoadLevel(string sceneName)
    {
        this.loadingScreen.SetActive(true);
        UpdateGameManager();
        StartCoroutine(LoadAsynchronously(sceneName));
    }

    private void UpdateGameManager()
    {
        // player data
        if (SceneManager.GetActiveScene().name != SceneNames.MAIN_MENU_SCENE || SceneManager.GetActiveScene().name != SceneNames.TUTORIAL_SCENE)
        {
            GameManager.Instance.UpdatePlayerData();
        }

        // debug data
        if (DebugHandler.Instance != null)
        {
            DebugHandler.Instance.UpdateDebugDataToGameManager();
        }
    }

    IEnumerator LoadAsynchronously(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            loadingScreenSlider.value = operation.progress;

            yield return null;
        }
    }
}
