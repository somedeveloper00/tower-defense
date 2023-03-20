using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TowerDefense.Core.Enemies;
using TowerDefense.ScriptUtils;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Core.Spawn {
    public class EnemySpawner : MonoBehaviour {
        public EnemyDatabase esd;
        public Transform instantiateParent;
        public SpawningMethod spawningMethod;

        void OnEnable() {
            spawningMethod.OnSpawnIn += (t, name, argvs) => {
                StartCoroutine( spawnAfter( name, t, (enemy) => {
                    foreach (var argv in argvs)
                        argv.TryApplyToEnemy( enemy );
                } ) );
            };
        }

        void Update() {
            spawningMethod.Tick( Time.deltaTime );
        }

        IEnumerator spawnAfter(string name, float t, Action<Enemy> afterSpawn) {
            yield return new WaitForSeconds( t );
            var enem = Spawn( name );
            afterSpawn?.Invoke( enem );
        }

        public Enemy Spawn(string name) {
            var prefab = esd.GetEnemyPrefab( name );
            var enem = Instantiate( prefab, instantiateParent );
            Debug.Log( $"\"{name}\" spawned {enem.name}" );
            return enem;
        }

        [Serializable]
        public class SpawningMethod {
            public List<OnTimeSpawn> onTime = new();
            
            public delegate void SpawnIn(float time, string name, EnemyArgManip[] argv);
            public SpawnIn OnSpawnIn;
            
            float _t = 0;

            [Serializable]
            public class OnTimeSpawn {
                public float time;
                public SpawnContainer[] spawns;

                [Serializable, DeclareHorizontalGroup( "h" )]
                public class SpawnContainer {

                    [Dropdown(nameof(nameOptions))]
                    public string name;

                    string[] nameOptions => EnemyDatabase.Instance.GetAllNames();

                    [Group( "h" ), LabelWidth( 40 ), Min( 0 )]
                    public int count;

                    [Group( "h" ), LabelWidth( 100 ), DisableIf( nameof(count), 0 )]
                    public float timeIntervals;

                    [SerializeReference, JsonConverter( typeof(SubTypeJsonConverter<EnemyArgManip>) )]
                    public EnemyArgManip[] argv;
                }
            }

            public void Tick(float deltaTime) {
                float newT = _t + deltaTime;
                foreach (var item in onTime) {
                    if ( newT > item.time && _t <= item.time ) {
                        foreach (var spawn in item.spawns) {
                            for (int i = 0; i < spawn.count; i++) {
                                OnSpawnIn?.Invoke( spawn.timeIntervals * i, spawn.name, spawn.argv );
                            }
                        }
                    }
                }

                _t = newT;
            }
        }
    }
}