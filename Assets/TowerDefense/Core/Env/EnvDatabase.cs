using System;
using System.Collections.Generic;
using System.Linq;
using TriInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerDefense.Core.Env {
    [CreateAssetMenu( fileName = "EnvDatabase", menuName = "Core/EnvDatabase", order = 0 )]
    public class EnvDatabase : ScriptableObject {
        
        public static EnvDatabase Instance;
        void OnEnable() => Instance = this;

        [SerializeField, TableList] List<Environment> environments;

        public string GetScenePath(string envName) => environments.Find( s => s.name == envName ).scenePath;

        public string[] GetAllNames() => environments.Select( e => e.name ).ToArray();

        [Serializable]
        public class Environment {
            public string name;
            [Scene] public string scenePath;
        }
    }
}