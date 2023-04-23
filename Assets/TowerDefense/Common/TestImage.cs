using System;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Common {
    public class TestImage : MonoBehaviour {
        [SerializeField] Image img;
        [SerializeField] GraphicRaycaster raycaster;
        void Update() {
            img.enabled = raycaster.enabled;
        }
    }
}