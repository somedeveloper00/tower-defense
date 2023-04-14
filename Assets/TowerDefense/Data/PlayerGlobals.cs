using System.Collections.Generic;
using TowerDefense.Background;
using TowerDefense.Core.Starter;
using TowerDefense.Data.Database;
using TowerDefense.Data.Progress;
using UnityEngine;

namespace TowerDefense.Data {
    [CreateAssetMenu( fileName = "PlayerGlobals", menuName = "TD/Player Globals", order = 0 )]
    public class PlayerGlobals : ScriptableObject {

        public static PlayerGlobals Current;
        void OnEnable() {
            Current = this;
            GameInitializer.onSecureDataLoad += Load;
        }


        public LevelsData levelsData;
        public EcoProgress ecoProg;
        public DefendersProgress defendersProg = new ();
        public LevelProgress levelProg = new ();


        public void Load(SecureDatabase secureDatabase) {
            if (secureDatabase.TryGetValue<DefendersProgress>( "defprog", out var defendersProg ))
                this.defendersProg = defendersProg;
            if (secureDatabase.TryGetValue<LevelProgress>( "lvlprog", out var levelProg ))
                this.levelProg = levelProg;
            if (secureDatabase.TryGetValue<EcoProgress>( "ecoprog", out var ecoProg ))
                this.ecoProg = ecoProg;

            // making sure level 1 is always unlocked
            GetOrCreateLevelProg( levelsData.coreLevels[0].id ).status |= LevelProgress.LevelStatus.Unlocked;
            
            Debug.Log( $"player global data loaded" );
        }

        public void Save(SecureDatabase secureDatabase) {
            secureDatabase.Set( "defprog", defendersProg );
            secureDatabase.Set( "lvlprog", levelProg );
            secureDatabase.Set( "ecoprog", ecoProg );
            
            secureDatabase.Save();
            Debug.Log( $"player global data saved" );
        }

        public bool TryGetLevelData(int id, out CoreLevelData level) {
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

        public LevelProgress.Level GetOrCreateLevelProg(int id) {
            for (int i = 0; i < levelProg.levels.Count; i++) {
                if (levelProg.levels[i].id == id) return levelProg.levels[i];
            }

            var lvl = new LevelProgress.Level();
            lvl.id = id;
            levelProg.levels.Add( lvl );
            return lvl;
        }

        public bool TryGetNextLevelProg(int id, out LevelProgress.Level level) {
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