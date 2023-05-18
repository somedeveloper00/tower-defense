using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameAnalyticsSDK;
using TapsellPlusSDK;
using UnityEngine;

namespace TowerDefense.Bridges.Ad {
    class TapsellAdManager : AdManager {
        
        const int RETRY_COUNT = 10;
        const string AD_SDK_NAME = "tapsell";
        
        bool initialized = false;
        Dictionary<string, string> activeBanners = new();

        public override async Task<bool> Initialize() {
            int tryCount = 0;
            
            bool canPass = false;
            bool result = false;
            try_init:
            canPass = false;
            result = false;
            tryCount++;
            
            TapsellPlus.Initialize( "krhsbokpomptalscqqhqqsfmiohccmqdkhcmbrfmerltfoeanjqbnkrgoetejptjbatomi",
                adNetworkName => {
                    logSensitive( "Tapsell ad Initialized Successfully: " + adNetworkName );
                    canPass = true;
                    result = true;
                },
                error => {
                    Debug.Log( error.ToString() );
                    canPass = true;
                    result = false;
                } );
            while (!canPass) await Task.Yield();
            
            // retry if failed
            if (!result) {
                if (tryCount < RETRY_COUNT) {
                    Debug.Log( $"failed. retrying..." );
                    goto try_init;
                }
                else {
                    Debug.Log( $"failed. max retried reached. giving up" );
                }
            }
            else {
                TapsellPlus.SetGdprConsent( true );
                initialized = true;
            }
            
            return result;
        }

        public override Task<bool> IsInitialized() => Task.FromResult( initialized );

        public override async Task ShowFullScreenBannerAd(string adId) {
            
            int tryCount = 0;
            
            request_try:
            tryCount++;
            bool canPass = false;
            string responseId = null;

            TapsellPlus.RequestInterstitialAd( adId,
                onRequestResponse: model => {
                    GameAnalytics.NewAdEvent( GAAdAction.Loaded, GAAdType.Interstitial, AD_SDK_NAME, adId );
                    responseId = model.responseId;
                    canPass = true;
                },
                onRequestError: _ => {
                    canPass = true;
                } );
            while (!canPass) await Task.Yield();
            
            // retry if failed
            if (responseId is null) {
                if (tryCount < RETRY_COUNT) {
                    Debug.Log( $"failed. retrying..." );
                    goto request_try;
                }
                else {
                    Debug.Log( $"failed. max retried reached. giving up" );
                    GameAnalytics.NewAdEvent( GAAdAction.FailedShow, GAAdType.Interstitial, AD_SDK_NAME, adId );
                    return;
                }
            }
            
            
            // show ad
            tryCount = 0;
            bool success;
            show_try:
            tryCount++;
            canPass = false;
            success = false;
            
            TapsellPlus.ShowInterstitialAd( responseId,
                onAdOpened: _ => {
                    logSensitive( $"fullscreen tapsell ad opened: {responseId}" );
                    GameAnalytics.NewAdEvent( GAAdAction.Show, GAAdType.Interstitial, AD_SDK_NAME, adId );
                },
                onAdClosed: _ => {
                    logSensitive( $"fullscreen tapsell ad closed: {responseId}" );
                    canPass = true;
                    success = true;
                },
                onShowError: model => {
                    Debug.LogError( model.errorMessage );
                    success = false;
                    canPass = true;
                } );
            while (!canPass) await Task.Yield();
            
            // retry if failed
            if (!success) {
                if (tryCount < RETRY_COUNT) {
                    Debug.Log( $"failed. retrying..." );
                    goto show_try;
                }
                else {
                    Debug.Log( $"failed. max retried reached. giving up" );
                    GameAnalytics.NewAdEvent( GAAdAction.FailedShow, GAAdType.Interstitial, AD_SDK_NAME, adId );
                    return;
                }
            }
        }

        public override async Task ShowFullScreenVideoAd(string adId) {
            int tryCount = 0;
            
            request_try:
            tryCount++;
            bool canPass = false;
            string responseId = null;

            TapsellPlus.RequestInterstitialAd( adId,
                onRequestResponse: model => {
                    GameAnalytics.NewAdEvent( GAAdAction.Loaded, GAAdType.Video, AD_SDK_NAME, adId );
                    responseId = model.responseId;
                    canPass = true;
                },
                onRequestError: _ => {
                    canPass = true;
                } );
            while (!canPass) await Task.Yield();
            
            // retry if failed
            if (responseId is null) {
                if (tryCount < RETRY_COUNT) {
                    Debug.Log( $"failed. retrying..." );
                    goto request_try;
                }
                else {
                    Debug.Log( $"failed. max retried reached. giving up" );
                    GameAnalytics.NewAdEvent( GAAdAction.FailedShow, GAAdType.Video, AD_SDK_NAME, adId );
                    return;
                }
            }
            
            
            // show ad
            tryCount = 0;
            bool success;
            show_try:
            tryCount++;
            canPass = false;
            success = false;
            
            TapsellPlus.ShowInterstitialAd( responseId,
                onAdOpened: _ => {
                    logSensitive( $"fullscreen tapsell ad opened: {responseId}" );
                    GameAnalytics.NewAdEvent( GAAdAction.Show, GAAdType.Video, AD_SDK_NAME, adId );
                },
                onAdClosed: _ => {
                    logSensitive( $"fullscreen tapsell ad closed: {responseId}" );
                    canPass = true;
                    success = true;
                },
                onShowError: model => {
                    Debug.LogError( model.errorMessage );
                    success = false;
                    canPass = true;
                } );
            while (!canPass) await Task.Yield();
            
            // retry if failed
            if (!success) {
                if (tryCount < RETRY_COUNT) {
                    Debug.Log( $"failed. retrying..." );
                    goto show_try;
                }
                else {
                    Debug.Log( $"failed. max retried reached. giving up" );
                    GameAnalytics.NewAdEvent( GAAdAction.FailedShow, GAAdType.Video, AD_SDK_NAME, adId );
                    return;
                }
            }
        }

        public override async Task ShowSidedBannerAd(string adId) {
            
            int tryCount = 0;

            trying:
            tryCount++;
            string responseId = null;
            bool canPass = false;

            TapsellPlus.RequestStandardBannerAd( adId, BannerType.Banner320X50,
                tapsellPlusAdModel => {
                    GameAnalytics.NewAdEvent( GAAdAction.Loaded, GAAdType.Banner, AD_SDK_NAME, adId );
                    responseId = tapsellPlusAdModel.responseId;
                    canPass = true;
                },
                error => {
                    Debug.Log( "Error " + error.message );
                    canPass = true;
                }
            );
            while (!canPass) await Task.Yield();
            
            // retry if failed
            if (responseId is null) {
                if (tryCount < RETRY_COUNT) {
                    Debug.Log( $"failed. retrying..." );
                    goto trying;
                }
                Debug.Log( $"failed. max retried reached. giving up" );
                GameAnalytics.NewAdEvent( GAAdAction.FailedShow, GAAdType.Banner, AD_SDK_NAME, adId );
                return;
            }
            
            // show ad
            canPass = false;
            TapsellPlus.ShowStandardBannerAd( responseId, TapsellPlusSDK.Gravity.Bottom, TapsellPlusSDK.Gravity.Bottom,
                onAdOpened: _ => {
                    canPass = true;
                    GameAnalytics.NewAdEvent( GAAdAction.Show, GAAdType.Banner, AD_SDK_NAME, adId );
                    try {
                        activeBanners.Add( adId, responseId );
                        logSensitive( $"tapsell sided banner ad openned: {responseId}" );
                    }
                    catch (Exception e) {
                        Debug.LogError( e );
                    }
                }, onShowError: model => {
                    canPass = true;
                    Debug.LogError( model.errorMessage );
                    GameAnalytics.NewAdEvent( GAAdAction.FailedShow, GAAdType.Banner, AD_SDK_NAME, adId );
                } );
            while (!canPass) await Task.Delay( 10 );
        }

        public override Task<bool> IsSidedBannerAdShowing(string adId) {
            return Task.FromResult( activeBanners.ContainsKey( adId ) );
        }
        
        public override Task RemoveSidedBannerAd(string adId) {
            if (activeBanners.Remove( adId, out var responseId )) {
                TapsellPlus.DestroyStandardBannerAd( responseId );
                logSensitive( $"tapsell sided ad removed: {responseId}" );
            }

            return Task.FromResult( 0 );
        }

        public override async Task<RewardAdResult> ShowFullScreenRewardVideoAd(string adId) {
            int tryCount = 0;
            trying:
            tryCount++;
            string responseId = null;
            bool canPass = false;
            TapsellPlus.RequestRewardedVideoAd( adId,
                (response) => {
                    GameAnalytics.NewAdEvent( GAAdAction.Loaded, GAAdType.RewardedVideo, AD_SDK_NAME, adId );
                    logSensitive( $"ad request response: {response.responseId}" );
                    responseId = response.responseId;
                    canPass = true;
                },
                (error) => {
                    Debug.LogError( error.message );
                    canPass = true;
                });

            while (!canPass) await Task.Yield();
            
            if (responseId == null) {
                if (tryCount < 10) {
                    Debug.Log( $"failed. retrying..." );
                    goto trying;
                }
                Debug.Log( $"failed. max retried reached. giving up" );
                GameAnalytics.NewAdEvent( GAAdAction.FailedShow, GAAdType.RewardedVideo, AD_SDK_NAME, adId );
                return RewardAdResult.Fail;
            }


            // show ad
            tryCount = 0;
            RewardAdResult result = RewardAdResult.Fail;
            show_try:
            tryCount++;
            canPass = false;
            
            TapsellPlus.ShowRewardedVideoAd( responseId,
                onAdOpened: _ => {
                    Debug.Log( $"tapsel rewarder ad opened." );   
                    GameAnalytics.NewAdEvent( GAAdAction.Show, GAAdType.RewardedVideo, AD_SDK_NAME, adId );
                },
                onAdClosed: _ => {
                    canPass = true;
                    // user could close the ad after success
                    if (result != RewardAdResult.Success)
                        result = RewardAdResult.CancelByUser; 
                    logSensitive( $"tapsel rewarded ad closed: {result}" );   
                },
                onShowError: (errorModel) => {
                    Debug.LogError( $"ad failed: {errorModel.errorMessage}" );
                    canPass = true;
                    result = RewardAdResult.Fail;
                },
                onAdRewarded: _ => {
                    result = RewardAdResult.Success;
                    canPass = true;
                    logSensitive( $"tapsel rewarded ad successful: {result}" );   
                    GameAnalytics.NewAdEvent( GAAdAction.RewardReceived, GAAdType.RewardedVideo, AD_SDK_NAME, adId );
                });
            while (!canPass) await Task.Yield();
            
            // retry if failed
            if (result == RewardAdResult.Fail) {
                if (tryCount < RETRY_COUNT) {
                    Debug.Log( $"failed. retrying..." );
                    goto show_try;
                } 
                Debug.Log( $"failed. max retried reached. giving up" );
                GameAnalytics.NewAdEvent( GAAdAction.FailedShow, GAAdType.RewardedVideo, AD_SDK_NAME, adId );
            }
            
            return result;
        }

        void logSensitive(string message) {
#if !HIDE_SENSITIVE
            Debug.Log( message );
#endif
        }
    }
}