using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TowerDefense.Core.Enemies;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Core.EnemySpawn {
    public class EnemySpawner : MonoBehaviour {
        public EnemyDatabase esd;
        public Transform instantiateParent;
        public SpawningMethod spawningMethod;

        void Start() {
            spawningMethod.OnSpawnIn += (t, name, argvs) => {
                StartCoroutine( spawnAfter( name, t, (enemy) => {
                    foreach (var argv in argvs)
                        argv.TryApplyToEnemy( enemy );
                } ) );
            };
            spawningMethod.ResetTime();
        }

        void Update() {
            spawningMethod.Tick( Time.deltaTime );
        }

        IEnumerator spawnAfter(string name, float t, Action<Enemy> afterSpawn) {
            yield return new WaitForSeconds( t );
            var enem = Spawn( name );
            afterSpawn?.Invoke( enem );
        }

        public bool IsDone() {
            foreach (var t in spawningMethod.onTime) {
                foreach (var s in t.spawns) {
                    if (spawningMethod.t < (s.count - 1) * s.timeIntervals) return false;
                }
            }

            return true;
        }

        public Enemy Spawn(string name) {
            var prefab = esd.GetEnemyPrefab( name );
            var enem = Instantiate( prefab, instantiateParent );
            return enem;
        }

        [Serializable]
        public class SpawningMethod {
            public List<OnTimeSpawn> onTime = new();

            public delegate void SpawnIn(float afterSeconds, string name, EnemyArgManip[] argv);
            [JsonIgnore]
            public SpawnIn OnSpawnIn;
            
            [NonSerialized] public float t = 0;

            public void ResetTime() => t = 0;

            [Serializable]
            public class OnTimeSpawn {
                public float time;
                public SpawnContainer[] spawns;

                [Serializable, DeclareHorizontalGroup( "h" )]
                public class SpawnContainer {

                    [Dropdown(nameof(nameOptions))]
                    public string name;

                    string[] nameOptions => EnemyDatabase.Current.GetAllNames();

                    [Group( "h" ), LabelWidth( 40 ), Min( 0 )]
                    public int count;

                    [Group( "h" ), LabelWidth( 90 ), DisableIf( nameof(count), 0 )]
                    public float timeIntervals;

                    [SerializeReference]
                    public EnemyArgManip[] argv = Array.Empty<EnemyArgManip>();
                }
            }

            public void Tick(float deltaTime) {
                float newT = t + deltaTime;
                foreach (var item in onTime) {
                    if ( newT > item.time && t <= item.time ) {
                        foreach (var spawn in item.spawns) {
                            for (int i = 0; i < spawn.count; i++) {
                                OnSpawnIn?.Invoke( spawn.timeIntervals * i, spawn.name, spawn.argv );
                            }
                        }
                    }
                }

                t = newT;
            }
        }
    }
}