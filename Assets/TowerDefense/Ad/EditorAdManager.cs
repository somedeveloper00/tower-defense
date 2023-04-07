using System.Threading.Tasks;
using UnityEngine;

namespace TowerDefense.Ad {
    public class EditorAdManager : AdManager {
        public override async Task<bool> Initialize() {
            await Task.Delay( 2 );
            return true;
        }

        public override Task<bool> IsInitialized() => Task.FromResult( true );

        public override Task ShowSidedBannerAd(string adId) {
            Debug.Log( $"mock. showing sided banned ad" );
            return Task.Delay( 1 );
        }

        public override Task RemoveSidedBannerAd(string adId) {
            Debug.Log( $"mock. removing sided banned ad" );
            return Task.Delay( 1 );
        }

        public override Task ShowFullScreenBannerAd(string adId) {
            Debug.Log( $"mock. showing fullscreen banned ad" );
            return Task.Delay( 1 );
        }

        public override Task ShowFullScreenVideoAd(string adId) {
            Debug.Log( $"mock. showing fullscreen video ad" );
            return Task.Delay( 1 );
        }

        public override Task<RewardAdResult> ShowFullScreenRewardVideoAd(string adId) {
            Debug.Log( $"mock. showing fullscreen reward video ad" );
            return Task.FromResult( RewardAdResult.Success );
        }
    }
}