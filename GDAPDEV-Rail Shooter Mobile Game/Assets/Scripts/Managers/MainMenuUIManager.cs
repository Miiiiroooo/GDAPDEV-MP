using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuContainer;
    [SerializeField] private GameObject debugMenuContainer;

    void Start()
    {
        this.mainMenuContainer.SetActive(true);
        this.debugMenuContainer.SetActive(false);
    }

    public void OnPlayButton()
    {
        mainMenuContainer.SetActive(false);
        SceneLoader.Instance.LoadLevel(SceneNames.TUTORIAL_SCENE);
    }

    public void OnDebugButton()
    {
        mainMenuContainer.SetActive(false);
        debugMenuContainer.SetActive(true);
    }

    public void OnCloseButton()
    {
        mainMenuContainer.SetActive(true);
        debugMenuContainer.SetActive(false);
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
