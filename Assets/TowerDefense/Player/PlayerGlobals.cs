using TowerDefense.Player.Database;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Player {
    [CreateAssetMenu( fileName = "PlayerGlobals", menuName = "TD/Player Globals", order = 0 )]
    public class PlayerGlobals : ScriptableObject {

        public static PlayerGlobals Current;
        void OnEnable() => Current = this;

        public PlayerData playerData;
        public GameLevelsData gameLevelsData;

        [Button]
        public void Load() {
            playerData = SecureDatabase.Current.GetValue<PlayerData>( "playerData" );
            gameLevelsData = SecureDatabase.Current.GetValue<GameLevelsData>( "gameData" );
            Debug.Log( $"player global data loaded" );
        }

        [Button]
        public void Save() {
            SecureDatabase.Current.Set( "playerData", playerData );
            SecureDatabase.Current.Set( "gameData", gameLevelsData );
            SecureDatabase.Current.Save();
            Debug.Log( $"player global data saved" );
        }
    }
}