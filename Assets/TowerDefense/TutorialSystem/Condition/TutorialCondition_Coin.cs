using TowerDefense.Data;
using TowerDefense.Data.Progress;
using UnityEngine;

namespace TowerDefense.TutorialSystem {
    [CreateAssetMenu(menuName = "TD/Tutorial Conditions/Coin")]
    public class TutorialCondition_Coin : TutorialCondition {
        [SerializeField] long coins;
        [SerializeField] ConditionStatement condition;
        public override bool IsMet() => condition.Matches( PlayerGlobals.Current.ecoProg.Coins, coins );
    }
}