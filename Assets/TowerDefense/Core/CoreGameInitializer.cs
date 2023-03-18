using System;
using UnityEngine;

namespace TowerDefense.Core {
    [CreateAssetMenu( fileName = "CoreGameInitializer", menuName = "Core/Game Initializer", order = 0 )]
    public class CoreGameInitializer : ScriptableObject {


        [Serializable]
        public class EnemySpawn {
        }
    }
}