using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;


public class WebHandler : MonoBehaviour
{
    #region singleton
    public static WebHandler Instance { get; private set; }

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
        }
    }

    private void OnDestroy()
    {
        if (Instance != null && Instance == this)
        {
            Destroy(this.gameObject);
        }
    }
    #endregion

    private const int GROUP_NUM = 2;
    private const string GROUP_NAME = "Team Mecha";
    private const string SECRET_PASSWORD = "MechaIsTheMeta"; // DO NOT CHANGE
    private bool isUploadingNewPlayer = false;

    public string BaseURL
    {
        get { return "https://gdapdev-web-api.herokuapp.com/api/"; }
    }


    // Create Group
    public void CreateGroupToServer(Button button = null)
    {
        StartCoroutine(PostNewGroup(button));
    }

    private IEnumerator PostNewGroup(Button button)
    {
        // Disable button
        if (button != null)
            button.interactable = false;


        // Dictionary to contain the parameters to create a group
        Dictionary<string, object> groupParams = new Dictionary<string, object>();
        groupParams.Add("group_num", GROUP_NUM);
        groupParams.Add("group_name", GROUP_NAME);
        groupParams.Add("game_name", GameManager.GAME_NAME);
        groupParams.Add("secret", SECRET_PASSWORD);


        string requestString = JsonConvert.SerializeObject(groupParams);   // Turn the dicitonary into a JSON string
        byte[] requestData = Encoding.UTF8.GetBytes(requestString);        // Convert string into bytes


        using (UnityWebRequest request = new UnityWebRequest(BaseURL + "groups", "POST"))
        {
            request.SetRequestHeader("Content-Type", "application/json");  // Set what type of data is in the request
            request.uploadHandler = new UploadHandlerRaw(requestData);     // Add the request data using UploadHandler
            request.downloadHandler = new DownloadHandlerBuffer();         // Create a receiver for the response later
            
            yield return request.SendWebRequest();                         // When ready, send the web request


            if (string.IsNullOrEmpty(request.error))
            {
                string displayText = "";

                if (request.downloadHandler.text == "OK")
                    displayText = $"{request.responseCode}: {request.downloadHandler.text}\nSuccessfully added new group!";
                else if (request.downloadHandler.text == "ALREADY_EXISTS")
                    displayText = $"{request.responseCode}: {request.downloadHandler.text}\nGroup already exists!";

                OutputDisplayHandler.Instance.DisplayText(displayText);
                //Debug.Log($"Message: {request.downloadHandler.text}");
            }
            else
            {
                string displayText = $"{request.responseCode}: {request.error}\nFailed to add new group.";
                OutputDisplayHandler.Instance.DisplayText(displayText);
                //Debug.Log($"Error: {request.error}");
            }
        }

        groupParams.Clear();
        if (button != null)
            button.interactable = true;
    }


    // Create New HighScore
    public void CreateNewPlayerHighScore(string userName, float score, bool isDebugMode, Button button = null)
    {
        StartCoroutine(PostNewHighScore(userName, score, isDebugMode, button));
    }

    private IEnumerator PostNewHighScore(string userName, float score, bool isDebugMode, Button button)
    {
        if (button != null)
            button.interactable = false;

        Dictionary<string, object> playerParams = new Dictionary<string, object>();
        playerParams.Add("group_num", GROUP_NUM);
        playerParams.Add("user_name", userName);
        playerParams.Add("score", score);


        string requestString = JsonConvert.SerializeObject(playerParams);
        byte[] requestData = Encoding.UTF8.GetBytes(requestString);

        using(UnityWebRequest request = new UnityWebRequest(BaseURL + "scores", "POST"))
        {
            this.isUploadingNewPlayer = true;
            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = new UploadHandlerRaw(requestData);
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();


            if (isDebugMode)
            {
                if (string.IsNullOrEmpty(request.error))
                {
                    string displayText = "";

                    if (request.downloadHandler.text == "OK")
                        displayText = $"{request.responseCode}: {request.downloadHandler.text}\nSuccessfully added new player!";
                    else if (request.downloadHandler.text == "ALREADY_EXISTS")
                        displayText = $"{request.responseCode}: {request.downloadHandler.text}\nPlayer already exists!";

                    OutputDisplayHandler.Instance.DisplayText(displayText);
                }
                else
                {
                    string displayText = $"{request.responseCode}: {request.error}\nFailed to add new player.";
                    OutputDisplayHandler.Instance.DisplayText(displayText);
                }
            }

            this.isUploadingNewPlayer = false;
        }

        playerParams.Clear();
        if (button != null)
            button.interactable = true;
    }


    // Get leaderboard scores
    public void GetLeaderboardScores(Button button = null)
    {
        StartCoroutine(GetScores(button));
    }

    private IEnumerator GetScores(Button button)
    {
        if (button != null)
            button.interactable = false;

        while (this.isUploadingNewPlayer)
        {
            yield return null;
        }

        using (UnityWebRequest request = new UnityWebRequest(BaseURL + "groups/" + GROUP_NUM.ToString(), "GET"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();


            if (string.IsNullOrEmpty(request.error))
            {
                List<Dictionary<string, object>> leaderboard = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(request.downloadHandler.text);
                OutputDisplayHandler.Instance.DisplayLeaderboard(leaderboard);
            }
            else
            {
                string displayText = $"{request.responseCode}: {request.error}\nFailed to get the leaderboard scores.";
                OutputDisplayHandler.Instance.DisplayText(displayText);
            }
        }

        if (button != null)
            button.interactable = true;
    }

    // Reset leaderboard
    public void ResetLeaderboard(string secret, Button button = null)
    {
        StartCoroutine(DeleteCurrentLeaderBoard(secret, button));
    }

    private IEnumerator DeleteCurrentLeaderBoard(string secret, Button button)
    {
        if (button != null)
            button.interactable = false;


        Dictionary<string, object> requestParams = new Dictionary<string, object>();
        requestParams.Add("group_num", GROUP_NUM);
        requestParams.Add("secret", secret);


        string requestString = JsonConvert.SerializeObject(requestParams);
        byte[] requestData = Encoding.UTF8.GetBytes(requestString);

        using (UnityWebRequest request = new UnityWebRequest(BaseURL + "scores", "DELETE"))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = new UploadHandlerRaw(requestData);
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();


            if (string.IsNullOrEmpty(request.error))
            {
                string displayText = "";

                if (request.downloadHandler.text == "OK")
                    displayText = $"{request.responseCode}: {request.downloadHandler.text}\nSuccessfully reset the leaderboard!";
                else if (request.downloadHandler.text == "SECRET_MISMATCH")
                    displayText = $"{request.responseCode}: {request.downloadHandler.text}\nFailed to reset the leaderboard.";

                OutputDisplayHandler.Instance.DisplayText(displayText);
            }
            else
            {
                string displayText = $"{request.responseCode}: {request.error}\nFailed to reset the leaderboard.";
                OutputDisplayHandler.Instance.DisplayText(displayText);
            }
        }

        requestParams.Clear();
        if (button != null)
            button.interactable = true;
    }
}
