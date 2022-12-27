using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener
{
    public static AdsManager Instance { get; private set; }

    [SerializeField] private string _androidGameID;
    [SerializeField] private string _iOSGameID;
    [SerializeField] private string _gameID;
    [SerializeField] bool _testMode = true;

    [SerializeField] private InterstitialAd interstitialAd;
    [SerializeField] private BannerAd bannerAd;
    [SerializeField] private RewardedAd rewardedAd;

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
        }

        InitializeAds();
    }

    public void InitializeAds()
    {
        _gameID = (Application.platform == RuntimePlatform.IPhonePlayer) ? _iOSGameID : _androidGameID;
        Advertisement.Initialize(_gameID, _testMode, true, this);
    }

    public void OnInitializationComplete()
    {
        //Debug.Log("Unity Ads Initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        //Debug.Log($"Unity Ads Initialization failed: {error.ToString()} - {message}");
    }

    public InterstitialAd GetInterstitialAd()
    {
        return this.interstitialAd;
    }

    public BannerAd GetBannerAd()
    {
        return this.bannerAd;
    }

    public RewardedAd GetRewardedAd()
    {
        return this.rewardedAd;
    }
}
