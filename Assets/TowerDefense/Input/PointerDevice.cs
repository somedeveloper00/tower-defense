using System;
using UnityEngine;

namespace TowerDefense.Input {
    public abstract class PointerDevice : MonoBehaviour {
        public abstract Vector2 GetPointerPos();
        public abstract bool GetPointerDown();
        public abstract bool GetPointerHeldDown();
        public abstract bool GetPointerUp();
        protected abstract bool IsSupported();

        public static PointerDevice Current;

        void OnEnable() {
            if (IsSupported()) {
                Current = this;
                Debug.Log( $"current pointer device: {GetType().Name}" );
            }
        }
    }
}