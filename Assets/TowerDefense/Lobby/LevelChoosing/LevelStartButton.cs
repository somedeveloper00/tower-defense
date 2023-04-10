using System;
using RTLTMPro;
using TowerDefense.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Lobby.LevelChoosing {
    public class LevelStartButton : MonoBehaviour {
        [SerializeField] Button playBtn;
        [SerializeField] RTLTextMeshPro title;

        [NonSerialized] public GameLevelsData.Level levelData;

        void Start() {
            playBtn.onClick.AddListener( onPlayButtonClick );
            playBtn.interactable = levelData.runtimeData.status.HasFlag( GameLevelsData.LevelStatus.Unlocked );
            title.text = levelData.gameData.title;
        }
        void onPlayButtonClick() {
            LobbyManager.Current.StartGame( levelData.gameData, null );
        }
    }
}