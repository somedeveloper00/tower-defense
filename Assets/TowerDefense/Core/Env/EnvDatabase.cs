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

        public Scene GetScene(string envName) =>
            SceneManager.GetSceneByPath( environments.Find( s => s.name == envName ).scenePath );

        public List<string> GetAllNames() => environments.Select( e => e.name ).ToList();

        [Serializable]
        public class Environment {
            public string name;
            [Scene] public string scenePath;
        }
    }
}