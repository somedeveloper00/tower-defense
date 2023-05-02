using System;
using System.IO;
using TowerDefense.Background;
using TowerDefense.Data.Database;
using TowerDefense.Data.Progress;
using TriInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TowerDefense.Data {
    [CreateAssetMenu( fileName = "PlayerGlobals", menuName = "TD/Player Globals", order = 0 )]
    public class PlayerGlobals : ScriptableObject {

        public static PlayerGlobals Current;
        void OnEnable() {
            Current = this;
            GameInitializer.afterSecureDataLoad += LoadData;
            GameInitializer.beforeSecureDataSave += SetData;
        }

        /// <summary>
        /// minumum amount of coin required for entering a level
        /// </summary>
        public const int MIN_COIN_FOR_LEVEL = 100;   

        public LevelsData levelsData;
        public EcoProgress ecoProg;
        public DefendersProgress defendersProg = new ();
        public LevelProgress levelProg = new ();
        

        [Button("Get Levels Automatically")]
        void edit_getLevelsFromFolder() {
            levelsData.coreLevels.Clear();
            foreach (var file in Directory.GetFiles( "Assets/TowerDefense/GeneralSOs/Levels/" )) {
                if (file.EndsWith( ".asset" ))
                    levelsData.coreLevels.Add( AssetDatabase.LoadAssetAtPath<CoreLevelData>( file ) );
            }

            levelsData.coreLevels.Sort( (l1, l2) => int.Parse( l1.id ).CompareTo( int.Parse( l2.id ) ) );
        }
        


        public void LoadData(SecureDatabase secureDatabase) {
            this.defendersProg = secureDatabase.TryGetValue<DefendersProgress>( "defprog", out var defendersProg )
                ? defendersProg
                : new DefendersProgress();
            this.levelProg = secureDatabase.TryGetValue<LevelProgress>( "lvlprog", out var levelProg )
                ? levelProg
                : new LevelProgress();
            this.ecoProg = secureDatabase.TryGetValue<EcoProgress>( "ecoprog", out var ecoProg )
                ? ecoProg
                : new EcoProgress();

            if (this.ecoProg.coins < 20) {
                this.ecoProg.coins = 20;
            }

            // making sure level 1 is always unlocked
            GetOrCreateLevelProg( levelsData.coreLevels[0].id ).status |= LevelProgress.LevelStatus.Unlocked;

            Debug.Log( $"player global data loaded" );
        }

        public void SetData(SecureDatabase secureDatabase) {
            secureDatabase.Set( "defprog", defendersProg );
            secureDatabase.Set( "lvlprog", levelProg );
            secureDatabase.Set( "ecoprog", ecoProg );
            Debug.Log( $"player global data set" );
        }

        public bool TryGetLevelData(string id, out CoreLevelData level) {
            for (int i = 0; i < levelsData.coreLevels.Count; i++) {
                if (levelsData.coreLevels[i].id == id) {
                    if (i < levelsData.coreLevels.Count - 1) {
                        level = levelsData.coreLevels[i];
                        return true;
                    }
                    level = null;
                    return false;

                }
            }
            level = null;
            return false;
        }

        public LevelProgress.Level GetOrCreateLevelProg(string id) {
            for (int i = 0; i < levelProg.levels.Count; i++) {
                if (levelProg.levels[i].id == id) return levelProg.levels[i];
            }

            var lvl = new LevelProgress.Level();
            lvl.id = id;
            levelProg.levels.Add( lvl );
            return lvl;
        }

        public bool TryGetNextLevelProg(string id, out LevelProgress.Level level) {
            for (int i = 0; i < levelsData.coreLevels.Count; i++) {
                if (levelsData.coreLevels[i].id == id) {
                    if (i < levelsData.coreLevels.Count - 1) {
                        var nextId = levelsData.coreLevels[i + 1].id;
                        level = GetOrCreateLevelProg( nextId );
                        return true;
                    }
                    level = null;
                    return false;

                }
            }
            level = null;
            return false;
        }
    }
}