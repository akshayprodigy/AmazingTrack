using UnityEngine;
using System;

public class AdsManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    string appID = "ca-app-pub-4008725507774847~6344065071";
    #if UNITY_ANDROID
    string bannerID = "ca-app-pub-3940256099942544/6300978111";
    string interstitialID = "ca-app-pub-3940256099942544/1033173712";
    string rewardedID = "ca-app-pub-3940256099942544/5224354917";
    string nativeID = "ca-app-pub-3940256099942544/2247696110";
    #endif

    // BannerView _bannerView;
    // InterstitialAd interstitialAd;
    // RewardedAd rewardedAd;

    #region RewardVideo
    public void RewwardedRewardVideo(int coins){
        int reward = coins;
    }
    #endregion

    void Start()
    {
        // MobileAds.RaiseAdEventsOnUnityMainThread = true;
        // MobileAds.Initialize(initStatus => { 
        //     Debug.Log("Initialization Complete");
        // });
    }

    #region Banner  

    // public void LoadBannerAds()
    // {
    //     CreateBannerAds();
    //     ListenBannerAdsEvents();
    //     // create an instance of a banner view first.
    //     if(_bannerView == null)
    //     {
    //         CreateBannerAds();
    //     }

    //     // create our request used to load the ad.
    //     var adRequest = new AdRequest();
    //     adRequest.Keywords.Add("unity-admob-sample");

    //     // send the request to load the ad.
    //     Debug.Log("Loading banner ad.");
    //     _bannerView.LoadAd(adRequest);
    // }
    // public void CreateBannerAds()
    // {
    //     if(_bannerView != null){
    //         DestroyBannerAds();
    //     }
    //     _bannerView = new BannerView(bannerID, AdSize.Banner, AdPosition.Bottom);
    // }

    // public void ListenBannerAdsEvents(){
    // // Raised when an ad is loaded into the banner view.
    //     _bannerView.OnBannerAdLoaded += () =>
    //     {
    //         Debug.Log("Banner view loaded an ad with response : "
    //             + _bannerView.GetResponseInfo());
    //     };
    //     // Raised when an ad fails to load into the banner view.
    //     _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
    //     {
    //         Debug.LogError("Banner view failed to load an ad with error : "
    //             + error);
    //     };
    //     // Raised when the ad is estimated to have earned money.
    //     _bannerView.OnAdPaid += (AdValue adValue) =>
    //     {
    //         Debug.Log(String.Format("Banner view paid {0} {1}.",
    //             adValue.Value,
    //             adValue.CurrencyCode));
    //     };
    //     // Raised when an impression is recorded for an ad.
    //     _bannerView.OnAdImpressionRecorded += () =>
    //     {
    //         Debug.Log("Banner view recorded an impression.");
    //     };
    //     // Raised when a click is recorded for an ad.
    //     _bannerView.OnAdClicked += () =>
    //     {
    //         Debug.Log("Banner view was clicked.");
    //     };
    //     // Raised when an ad opened full screen content.
    //     _bannerView.OnAdFullScreenContentOpened += () =>
    //     {
    //         Debug.Log("Banner view full screen content opened.");
    //     };
    //     // Raised when the ad closed full screen content.
    //     _bannerView.OnAdFullScreenContentClosed += () =>
    //     {
    //         Debug.Log("Banner view full screen content closed.");
    //     };
    // }

    // public void DestroyBannerAds(){
    //     if(_bannerView != null){
    //         _bannerView.Destroy();
    //         _bannerView = null;
    //     }
    // }

    #endregion

#region Interstitial

    public void LoadInterstitialAds(){

    }

    public void ShowInterstitialAds(){
    }

    public void CreateInterstitialAds(){
    }

    public void ListenInterstitialAdsEvents(){

    }

    #endregion
    // Update is called once per frame
    void Update()
    {
        
    }
}
