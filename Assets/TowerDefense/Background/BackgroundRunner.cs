using System;
using UnityEngine;

namespace TowerDefense.Background {
    public class BackgroundRunner : MonoBehaviour {
        public static BackgroundRunner Current;
        void OnEnable() => Current = this;
    }
}