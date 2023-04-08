using System.Collections.Generic;
using System.Threading.Tasks;
using TapsellPlusSDK;
using UnityEngine;

namespace TowerDefense.Ad {
    class TapsellAdManager : AdManager {

        const int RETRY_COUNT = 10;
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
                    Debug.Log( "Tapsell ad Initialized Successfully: " + adNetworkName );
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
                    responseId = model.responseId;
                    canPass = true;
                },
                onRequestError: error => {
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
                onAdOpened: model => {
                    // canPass = true;
                },
                onAdClosed: model => {
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
                    return;
                }
            }
        }

        public override Task ShowFullScreenVideoAd(string adId) => ShowFullScreenBannerAd( adId );

        public override async Task ShowSidedBannerAd(string adId) {
            
            int tryCount = 0;

            trying:
            tryCount++;
            string responseId = null;
            bool canPass = false;

            TapsellPlus.RequestStandardBannerAd( adId, BannerType.Banner320X50,
                tapsellPlusAdModel => {
                    Debug.Log( $"standard banned ad request success: {tapsellPlusAdModel.responseId}" );
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
                return;
            }
            
            // show ad
            TapsellPlus.ShowStandardBannerAd( responseId, TapsellPlusSDK.Gravity.Top, TapsellPlusSDK.Gravity.Top,
                onAdOpened: model => { }, onShowError: model => { } );
            activeBanners.Add( adId, responseId );
        }

        public override Task RemoveSidedBannerAd(string adId) {
            if (activeBanners.TryGetValue( adId, out var responseId ))
                TapsellPlus.DestroyStandardBannerAd( responseId );
            return null;
        }

        public override async Task<RewardAdResult> ShowFullScreenRewardVideoAd(string adId) {
            int tryCount = 0;
            trying:
            tryCount++;
            Debug.Log( $"requesting for interstitial ad" );
            string responseId = null;
            bool canPass = false;
            TapsellPlus.RequestRewardedVideoAd( adId,
                (response) => {
                    Debug.Log( $"ad request response: {response.responseId}" );
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
                return RewardAdResult.Fail;
            }


            // show ad
            tryCount = 0;
            RewardAdResult result = RewardAdResult.Fail;
            show_try:
            tryCount++;
            canPass = false;
            
            TapsellPlus.ShowRewardedVideoAd( responseId,
                onAdOpened: model => {
                    
                },
                onAdClosed: model => {
                    canPass = true;
                    result = RewardAdResult.CancelByUser;
                },
                onShowError: (errorModel) => {
                    Debug.LogError( $"ad failed: {errorModel.errorMessage}" );
                    canPass = true;
                    result = RewardAdResult.Fail;
                },
                onAdRewarded: model => {
                    canPass = true;
                    result = RewardAdResult.Success;
                });
            while (!canPass) await Task.Yield();
            
            // retry if failed
            if (result == RewardAdResult.Fail) {
                if (tryCount < RETRY_COUNT) {
                    Debug.Log( $"failed. retrying..." );
                    goto show_try;
                } 
                Debug.Log( $"failed. max retried reached. giving up" );
            }
            
            return result;
        }
    
    }
}