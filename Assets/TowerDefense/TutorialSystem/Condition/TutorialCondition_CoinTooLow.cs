using TowerDefense.Background;
using UnityEngine;

namespace TowerDefense.TutorialSystem {
    [CreateAssetMenu(menuName = "TD/Tutorial Conditions/Coin Too Low")]
    public class TutorialCondition_CoinTooLow : TutorialCondition {
        public override bool IsMet() => CoinIncreaseHelpSystem.Current && CoinIncreaseHelpSystem.Current.IsCoinBelowThreshold();
    }
}