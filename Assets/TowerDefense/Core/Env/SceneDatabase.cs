using System;
using System.Collections.Generic;
using System.Linq;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Core.Env {
    [CreateAssetMenu( fileName = "SceneDatabase", menuName = "Common/Scene Database", order = 0 )]
    public class SceneDatabase : ScriptableObject {

        public static SceneDatabase Instance;
        void OnEnable() => Instance = this;

        [SerializeField, TableList] List<SceneItem> environments;

        public string GetScenePath(string envName) => environments.Find( s => s.name == envName ).scenePath;

        public string[] GetAllNames() => environments.Select( e => e.name ).ToArray();

        [Serializable]
        public class SceneItem {
            public string name;
            [Scene] public string scenePath;
        }
    }
}