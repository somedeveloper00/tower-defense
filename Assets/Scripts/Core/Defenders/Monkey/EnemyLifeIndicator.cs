using System;
using Core.Enemies;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Defenders.Monkey {
    public class EnemyLifeIndicator : MonoBehaviour {
        public Enemy enemy;
        [SerializeField] Image image;

        void Awake() {
            enemy.onLifeChanged += _ => updateImage();
        }

        void updateImage() {
            var t = enemy.life / enemy.startingLife;
            var anchor = image.rectTransform.anchorMin;
            anchor.x = 1 - t;
            image.rectTransform.anchorMin = anchor;
        }
    }
}