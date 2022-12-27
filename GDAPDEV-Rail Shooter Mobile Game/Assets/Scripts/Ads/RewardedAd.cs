using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class RewardedAd : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private Button _showAdBtn;
    [SerializeField] private Button _watchAdBtn;
    [SerializeField] private string _androidAdUnitID = "Rewarded_Android";
    [SerializeField] private string _iOSAdUnitID = "Rewarded_iOS";
    [SerializeField] private string _adUnitID;
    private bool isDebug = true;

    void Awake()
    {
        _adUnitID = (Application.platform == RuntimePlatform.IPhonePlayer) ? _iOSAdUnitID : _androidAdUnitID;

        _showAdBtn.interactable = false;
    }

    void Start()
    {
        StartCoroutine(WaitForAdsManagerInitialized());
    }

    private IEnumerator WaitForAdsManagerInitialized()
    {
        yield return new WaitUntil(() => Advertisement.isInitialized);

        LoadAd();
    }

    public void LoadAd()
    {
        //Debug.Log("Loading Ad: " + _adUnitID);
        Advertisement.Load(_adUnitID, this);
    }

    public void ShowAd(bool isDebug)
    {
        this.isDebug = isDebug;

        if (isDebug)
        {
            _showAdBtn.interactable = false;
        }
        else
        {
            _watchAdBtn.interactable = false;
        }

        Advertisement.Show(_adUnitID);
    }


    // Methods for the Unity Ads Load Listener Interface
    public void OnUnityAdsAdLoaded(string placementID)
    {
        //Debug.Log($"{placementID} has been loaded");
        _showAdBtn.interactable = true;

        if (_watchAdBtn != null)
            _watchAdBtn.interactable = true;
    }

    public void OnUnityAdsFailedToLoad(string placementID, UnityAdsLoadError error, string message)
    {
        //Debug.Log($"{placementID} cannot be loaded: {error.ToString()} = {message}");
    }


    // Methods for the Unity Ads Show Listener Interface
    public void OnUnityAdsShowFailure(string placementID, UnityAdsShowError error, string message)
    {
        //Debug.Log($"Error showing ad unit {placementID}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string placementID)
    {

    }

    public void OnUnityAdsShowClick(string placementID)
    {

    }

    public void OnUnityAdsShowComplete(string placementID, UnityAdsShowCompletionState showCompletionState)
    {
        //Debug.Log("Unity ads show complete");
        //OutputDisplayHandler.Instance.DisplayText("Unity ads show complete");

        if (placementID.Equals(_adUnitID) && showCompletionState.Equals(UnityAdsCompletionState.COMPLETED))
        {
            //Debug.Log(!this.isDebug);

            //Debug.Log("Success");
            if(!this.isDebug)
            {
                GameManager.Instance.GetPlayerScript().OnRevive();
                GameManager.Instance.UpdateGameState(GameManager.GameStates.Gameplay);
            }
        }

        Advertisement.Load(_adUnitID, this);
    }
}