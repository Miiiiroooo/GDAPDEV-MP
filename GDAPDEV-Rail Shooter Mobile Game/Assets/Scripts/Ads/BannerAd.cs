using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class BannerAd : MonoBehaviour
{
    [SerializeField] private Button _showBannerBtn;
    [SerializeField] private Button _hideBannerBtn;
    [SerializeField] private string _androidAdUnitID = "Banner_Android";
    [SerializeField] private string _iOSAdUnitID = "Banner_iOS";
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

        LoadBanner();
    }

    public void LoadBanner()
    {
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Advertisement.Banner.SetPosition(BannerPosition.TOP_CENTER);
        Advertisement.Banner.Load(_adUnitID, options);
    }

    private void OnBannerLoaded()
    {
        //Debug.Log("Banner Loaded");
        _showBannerBtn.interactable = true;
    }

    private void OnBannerError(string message)
    {
        //Debug.Log($"Banner load error: {message}");
    }

    public void ShowBannerAd(bool isDebug)
    {
        //Debug.Log(Advertisement.isShowing);

        if (Advertisement.isShowing)
            return;

        BannerOptions options;
        if (isDebug)
        {
            options = new BannerOptions
            {
                clickCallback = OnBannerClicked,
                showCallback = OnBannerShownDebug,
                hideCallback = OnBannerHiddenDebug
            };
        }
        else
        {
            options = new BannerOptions
            {
                clickCallback = OnBannerClicked,
                showCallback = OnBannerShown,
                hideCallback = OnBannerHidden
            };
        }

        Advertisement.Banner.SetPosition(BannerPosition.TOP_CENTER);
        Advertisement.Banner.Show(_adUnitID, options);
    }

    public void OnBannerClicked()
    {

    }

    public void OnBannerShown()
    {
        
    }

    public void OnBannerHidden()
    {
       
    }

    public void OnBannerShownDebug()
    {
        _showBannerBtn.interactable = false;
        _hideBannerBtn.interactable = true;
    }

    public void OnBannerHiddenDebug()
    {
        _showBannerBtn.interactable = true;
        _hideBannerBtn.interactable = false;
    }

    public void HideBannerAd()
    {
        Advertisement.Banner.Hide(true);
    }
}
