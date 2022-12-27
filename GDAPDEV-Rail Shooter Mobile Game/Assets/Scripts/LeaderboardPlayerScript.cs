using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardPlayerScript : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI rankText;
    [SerializeField] private TMPro.TextMeshProUGUI nameText;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;

    public void SetRankText(int rank)
    {
        if (this.rankText.text != "0.")
            return;

        this.rankText.text = rank.ToString() + ".";
    }

    public void SetNameText(string text)
    {
        this.nameText.text = text;
    }

    public void SetScoreText(string score)
    {
        this.scoreText.text = score;
    }
}
