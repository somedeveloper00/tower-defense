using TowerDefense.Background;
using UnityEngine;

namespace TowerDefense.TutorialSystem {
    [CreateAssetMenu(menuName = "TD/Tutorial Conditions/Coin Ad Available")]
    public class TutorialCondition_CoinAdAvailable : TutorialCondition {
        public override bool IsMet() => CoinIncreaseHelpSystem.Current && CoinIncreaseHelpSystem.Current.CanWatchAd();
    }
}