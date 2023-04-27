using System.Threading.Tasks;

namespace TowerDefense.Bridges.Ad {

    public abstract class AdManager {

        public AdManager() => Current = this;
        
        public static AdManager Current;

        public abstract Task<bool> Initialize();

        public abstract Task<bool> IsInitialized();

        public abstract Task ShowSidedBannerAd(string adId);
        
        public abstract Task<bool> IsSidedBannerAdShowing(string adId);
            
        public abstract Task RemoveSidedBannerAd(string adId);
        
        public abstract Task ShowFullScreenBannerAd(string adId);
        
        public abstract Task ShowFullScreenVideoAd(string adId);
        
        public abstract Task<RewardAdResult> ShowFullScreenRewardVideoAd(string adId);

        public enum RewardAdResult {
            Success, CancelByUser, Fail
        }
    }
}