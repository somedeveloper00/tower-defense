using TowerDefense.Player.Database;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Player {
    [CreateAssetMenu( fileName = "PlayerGlobals", menuName = "TD/Player Globals", order = 0 )]
    public class PlayerGlobals : ScriptableObject {
        public PlayerData playerData;
        public GameData gameData;

        [Button]
        public void Load() {
            playerData = SecureDatabase.Current.GetValue<PlayerData>( "playerData" );
            gameData = SecureDatabase.Current.GetValue<GameData>( "gameData" );
            Debug.Log( $"player global data loaded" );
        }

        [Button]
        public void Save() {
            SecureDatabase.Current.Set( "playerData", playerData );
            SecureDatabase.Current.Set( "gameData", gameData );
            Debug.Log( $"player global data saved" );
        }
    }
}