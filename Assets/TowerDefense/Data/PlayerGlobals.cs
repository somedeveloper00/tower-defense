using TowerDefense.Background;
using TowerDefense.Data.Database;
using UnityEngine;

namespace TowerDefense.Data {
    [CreateAssetMenu( fileName = "PlayerGlobals", menuName = "TD/Player Globals", order = 0 )]
    public class PlayerGlobals : ScriptableObject {

        public static PlayerGlobals Current;
        void OnEnable() {
            Current = this;
            GameInitializer.onSecureDataLoad += Load;
        }

        public PlayerData playerData;
        public GameLevelsData gameLevelsData;

        
        public void Load(SecureDatabase secureDatabase) {
            playerData = secureDatabase.GetValue<PlayerData>( "playerData" );
            gameLevelsData = secureDatabase.GetValue<GameLevelsData>( "gameData" );
            Debug.Log( $"player global data loaded" );
        }

        public void Save(SecureDatabase secureDatabase) {
            secureDatabase.Set( "playerData", playerData );
            secureDatabase.Set( "gameData", gameLevelsData );
            secureDatabase.Save();
            Debug.Log( $"player global data saved" );
        }
    }
}