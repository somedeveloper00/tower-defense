using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Core.Enemies {
    public class EnemyLifeIndicator : MonoBehaviour {
        public Enemy enemy;
        [SerializeField] Image image;

        void Awake() {
            enemy.onLifeChanged += _ => updateImage();
        }

        void updateImage() {
            var t = enemy.life / enemy.startingLife;
            var anchor = image.rectTransform.anchorMax;
            anchor.x = t;
            image.rectTransform.anchorMax = anchor;
        }
    }
}