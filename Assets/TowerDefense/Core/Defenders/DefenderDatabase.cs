using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Core.Defenders {
    [CreateAssetMenu( fileName = "DefenderDatabase", menuName = "Core/Defender Database", order = 0 )]
    public class DefenderDatabase : ScriptableObject {

        [SerializeField] List<DefenderItem> defenderItems;

        public static DefenderDatabase Current;

        public Defender GetDefenderMainPrefab(string name) => defenderItems.Find( d => d.name == name ).mainPrefab;
        public GameObject GetDefenderPlaceholderPrefab(string name) => defenderItems.Find( d => d.name == name ).placeholderPrefab;

        void OnEnable() => Current = this;


        [Serializable]
        public class DefenderItem {
            [Title("$name")]
            public string name;
            public Defender mainPrefab;
            public GameObject placeholderPrefab;
        }
    }
}