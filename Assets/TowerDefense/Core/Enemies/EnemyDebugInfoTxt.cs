using System;
using TMPro;
using UnityEngine;

namespace TowerDefense.Core.Enemies
{
    public class EnemyDebugInfoTxt : MonoBehaviour
    {
        public Enemy enemy;
        public TMP_Text text;

        void LateUpdate() {
            updateText();
        }

        void updateText() {
            string msg = String.Empty;
            if ( enemy == null ) {
                msg = "enemy is null".SetHtmlColor( Color.red );
                text.text = msg;
            }

            msg += $"name: {enemy.name}".SetHtmlColor( Color.green );
            msg += $"\nhp: {enemy.life} / {enemy.startingLife}";
            msg += $"\ncompletion: {enemy.GetRoadCompletion().ToString("0.00")}";
            text.text = msg;
        }
    }
}