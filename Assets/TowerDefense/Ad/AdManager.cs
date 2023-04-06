using System.Collections;
using TapsellPlusSDK;
using UnityEngine;

namespace TowerDefense.Ad {
    public static class AdManager {

        // [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSplashScreen )]
        public static void Initialize() {
            Debug.Log( $"initializing tapsell ad" );
            TapsellPlus.Initialize( "krhsbokpomptalscqqhqqsfmiohccmqdkhcmbrfmerltfoeanjqbnkrgoetejptjbatomi",
                adNetworkName => Debug.Log( adNetworkName + " Initialized Successfully." ),
                error => Debug.Log( error.ToString() ) );
            TapsellPlus.SetGdprConsent( true );
        }

        public static IEnumerator ShowRewardedAd() {

            Debug.Log( $"requesting for interstitial ad" );
            string ad_id = null;
            bool canPass = true;
            TapsellPlus.RequestInterstitialAd( "642ee1bd2eeae447e5ae5bb3",
                (response) => {
                    Debug.Log( $"ad request response: {response.responseId}" );
                    ad_id = response.responseId;
                    canPass = true;
                },
                (error) => {
                    Debug.LogError( error.message );
                    canPass = true;
                });
            
            yield return new WaitUntil( () => canPass );
            if (ad_id == null) yield break;
            
            canPass = false;
            TapsellPlus.ShowInterstitialAd( ad_id,
                onAdOpened: (adModel) => { },
                onAdClosed: (adModel) => canPass = true,
                onShowError: (errorModel) => {
                    Debug.LogError( $"ad failed: {errorModel.errorMessage}" );
                    canPass = true;
                } );
            
            yield return new WaitUntil( () => canPass );
        }
    }
}