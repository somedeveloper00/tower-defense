using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TowerDefense.Core.DefenderSpawn {
    public class PointerAreaEvents : Image, IPointerDownHandler, IPointerUpHandler {

        public Action onPointerUp, onPointerDown;
        public bool isDown = false;

        public void OnPointerDown(PointerEventData eventData) {
            isDown = true;
            Debug.Log( "down" );
            onPointerDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData) {
            Debug.Log( "up" );
            isDown = false;
            onPointerUp?.Invoke();
        }
    }
}