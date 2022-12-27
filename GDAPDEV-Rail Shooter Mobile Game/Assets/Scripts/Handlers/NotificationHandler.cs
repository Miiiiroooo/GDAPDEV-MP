using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;

public class NotificationHandler : MonoBehaviour
{
    #region singleton and unity methods
    public static NotificationHandler Instance { get; private set; }

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
            GameObject.DontDestroyOnLoad(this.gameObject);
        }

        BuildGeneralNotificationChannel();
        BuildSocialNotificationChannel();
    }

    private void OnDestroy()
    {
        if (Instance != null && Instance == this)
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        //System.TimeSpan currentRealTime = new System.TimeSpan(System.DateTime.Now.Hour, System.DateTime.Now.Minute, System.DateTime.Now.Second);
        //System.TimeSpan timeSchedule1 = new System.TimeSpan(6, 0, 0);   // 6 AM && PM
        //System.TimeSpan timeSchedule2 = new System.TimeSpan(7, 0, 0);   // 7 AM && PM

        //if (!hasPostRegularNotif && (currentRealTime == timeSchedule1 || currentRealTime == timeSchedule2))
        //{
        //    hasPostRegularNotif = true;
        //    PostGeneralNotifications();
        //}
        //else if (currentRealTime != timeSchedule1 && currentRealTime != timeSchedule2)
        //{
        //    hasPostRegularNotif = false;
        //}
    }
    #endregion


    private const string GENERAL_CHANNEL_ID = "general";
    private const string SOCIAL_CHANNEL_ID = "social";
    private bool hasPostRegularNotif = false;

    private void BuildGeneralNotificationChannel()
    {
        string channelID = GENERAL_CHANNEL_ID;
        string title = "General Notifications";
        string description = "General notifications about the game";
        Importance importance = Importance.Default;

        AndroidNotificationChannel generalChannel = new AndroidNotificationChannel(channelID, title, description, importance);
        AndroidNotificationCenter.RegisterNotificationChannel(generalChannel);
    }

    private void BuildSocialNotificationChannel()
    {
        string channelID = SOCIAL_CHANNEL_ID;
        string title = "Social Notifications";
        string description = "Social notifications from the game";
        Importance importance = Importance.Default;

        AndroidNotificationChannel socialChannel = new AndroidNotificationChannel(channelID, title, description, importance);
        AndroidNotificationCenter.RegisterNotificationChannel(socialChannel);
    }

    public void PostGeneralNotifications(string message = "", int fireTimeInput = -1)
    {
        string notif_title;
        string notif_message;
        if (message == "")
        {
            notif_title = "MECHA MADNESS";
            notif_message = "Mecha's are now invading, and we need you to help us!\nCome play Mecha Madness now!";
        }
        else
        {
            notif_title = "MECHA GENERAL";
            notif_message = message;
        }

        System.DateTime fireTime;
        if (fireTimeInput != -1)
        {
            fireTime = System.DateTime.Now.AddSeconds(fireTimeInput);
        }
        else
        {
            fireTime = System.DateTime.Now.AddSeconds(5);
        }

        AndroidNotification notif = new AndroidNotification(notif_title, notif_message, fireTime);
        AndroidNotificationCenter.SendNotification(notif, GENERAL_CHANNEL_ID);
    }

    public void PostSocialNotifications(float score = -1.0f, string message = "", int fireTimeInput = -1)
    {
        string notif_title;
        string notif_message;
        if (score != -1.0f)
        {
            notif_title = "MECHA LEADERBOARD";
            notif_message = $"Someone has joined the leaderboard with a score of {score}!";
        }
        else if (message != "")
        {
            notif_title = "MECHA SOCIAL";
            notif_message = message;
        }
        else
        {
            notif_title = "MECHA SOCIAL";
            notif_message = "default message";
        }

        System.DateTime fireTime;
        if (fireTimeInput != -1)
        {
            fireTime = System.DateTime.Now.AddSeconds(fireTimeInput);
        }
        else
        {
            fireTime = System.DateTime.Now.AddSeconds(5);
        }

        AndroidNotification notif = new AndroidNotification(notif_title, notif_message, fireTime);
        AndroidNotificationCenter.SendNotification(notif, SOCIAL_CHANNEL_ID);
    }
}
