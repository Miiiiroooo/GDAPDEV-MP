using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }
    [SerializeField] private AudioSource audioScouce;
    [SerializeField] private float mainMenuVolume = 0.05f;
    [SerializeField] private float gameVolume = 0.02f;

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
            this.audioScouce = this.GetComponent<AudioSource>();
            GameObject.DontDestroyOnLoad(this.gameObject);
            SceneManager.activeSceneChanged += OnChangeScene;
        }
    }
    
    void OnDestroy()
    {
        if (Instance != null && Instance == this)
        {
            SceneManager.activeSceneChanged -= OnChangeScene;
            Destroy(this.gameObject);
        }
    }

    private void OnChangeScene(Scene currentScene, Scene nextScene)
    {
        if (nextScene.name == SceneNames.MAIN_MENU_SCENE)
        {
            this.audioScouce.volume = mainMenuVolume;
        }
        else 
        {
            this.audioScouce.volume = gameVolume;
        }
    }
}
