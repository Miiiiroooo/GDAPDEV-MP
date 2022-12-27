using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutputDisplayHandler : MonoBehaviour
{
    #region singleton and unity methods
    public static OutputDisplayHandler Instance { get; private set; }
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

        CloseDisplayMenu();
    }
    #endregion

    // fields
    [SerializeField] private GameObject outputDisplayContainer;
    [SerializeField] private int currentLeaderboardIndex = -1;

    // UI for displaying texts
    [SerializeField] private GameObject textDisplayMenu;
    [SerializeField] private TMPro.TextMeshProUGUI textDisplay;

    // UI for displaying the leaderboards
    [SerializeField] private GameObject leaderboardsDisplayMenu;
    [SerializeField] private GameObject leaderboardsPlayerCopy;
    [SerializeField] private GameObject leaderboardsPlayerParent;
    [SerializeField] private List<GameObject> leaderboardPlayersList;

    //UI for displaying ads
    //[SerializeField] private GameObject adsDisplayMenu;


    private void DisableAllOutputMenus()
    {
        this.textDisplayMenu.SetActive(false);
        this.leaderboardsDisplayMenu.SetActive(false);
    }

    public void DisplayText(string text)
    {
        this.outputDisplayContainer.SetActive(true);
        this.textDisplayMenu.SetActive(true);
        this.textDisplay.text = text;
    }

    public void DisplayLeaderboard(List<Dictionary<string, object>> leaderboard)
    {
        this.outputDisplayContainer.SetActive(true);

        ConvertWebDataToProperLeaderboardFormat(leaderboard);
        this.leaderboardsDisplayMenu.SetActive(true);

        this.currentLeaderboardIndex = -1;
    }

    public void DisplayAds()
    {

    }

    public void CloseDisplayMenu()
    {
        DisableAllOutputMenus();
        this.outputDisplayContainer.SetActive(false);
    }

    private void ConvertWebDataToProperLeaderboardFormat(List<Dictionary<string, object>> leaderboard)
    {
        if (this.leaderboardPlayersList.Count < leaderboard.Count)
        {
            int newPlayerIndex = this.leaderboardPlayersList.Count + 1;
            int difference = leaderboard.Count - this.leaderboardPlayersList.Count;

            for (int i = 0; i < difference; i++)
            {
                GameObject newPlayer = GameObject.Instantiate(leaderboardsPlayerCopy, this.leaderboardsPlayerParent.transform);
                LeaderboardPlayerScript script = newPlayer.GetComponent<LeaderboardPlayerScript>();
                script.SetRankText(newPlayerIndex + i);

                this.leaderboardPlayersList.Add(newPlayer);
            }
        }


        this.currentLeaderboardIndex = leaderboard.Count - 1;

        for (int i = 0; i < leaderboard.Count; i++)
        {
            LeaderboardPlayerScript script = this.leaderboardPlayersList[i].GetComponent<LeaderboardPlayerScript>();

            string name = leaderboard[this.currentLeaderboardIndex]["user_name"].ToString();
            script.SetNameText(name);

            float webScore = System.Convert.ToSingle(leaderboard[this.currentLeaderboardIndex]["score"]);
            string stringScore = ScoreManager.Instance.ConvertFloatScoreIntoStringScore(webScore);
            script.SetScoreText(stringScore);

            this.currentLeaderboardIndex--;
        }
    }
}
