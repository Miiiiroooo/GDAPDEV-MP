using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class InterstitialAd : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private string _androidAdUnitID = "Interstitial_Android";
    [SerializeField] private string _iOSAdUnitID = "Interstitial_iOS";
    [SerializeField] private string _adUnitID;

    void Awake()
    {
        _adUnitID = (Application.platform == RuntimePlatform.IPhonePlayer) ? _iOSAdUnitID : _androidAdUnitID;
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
        // Important! Only load when the Advertisements class has been initialized. Otherwise, loading an Ad will result to an error
        //Debug.Log("Loading Ad: " + _adUnitID);
        Advertisement.Load(_adUnitID, this);
    }

    public void ShowAd()
    {
        // Shows the ad. If the ad is not loaded, it will not be displayed
        //Debug.Log("Showing Ad: " + _adUnitID);
        Advertisement.Show(_adUnitID);

        // Reload the ad for next time it is shown
        LoadAd();
    }


    // Methods for the Unity Ads Load Listener Interface
    public void OnUnityAdsAdLoaded(string placementID)
    {
        //Debug.Log($"{placementID} has been loaded");
    }

    public void OnUnityAdsFailedToLoad(string placementID, UnityAdsLoadError error, string message)
    {
        //Debug.Log($"{placementID} failed to load: {error.ToString()} = {message}");
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

    }
}
