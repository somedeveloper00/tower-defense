using System;
using UnityEngine;

namespace Core {
    [CreateAssetMenu( fileName = "CoreGameInitializer", menuName = "Core/Game Initializer", order = 0 )]
    public class CoreGameInitializer : ScriptableObject {


        [Serializable]
        public class EnemySpawn {
        }
    }
}