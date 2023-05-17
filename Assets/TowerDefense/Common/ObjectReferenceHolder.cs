using TowerDefense.Core.Defenders;
using UnityEngine;

namespace TowerDefense {
    sealed class ObjectReferenceHolder : MonoBehaviour {
        [SerializeField] ScriptableObject[] objects;
    }
}