using System;
using RTLTMPro;
using TowerDefense.Core.Starter;
using TowerDefense.Data;
using TowerDefense.Data.Progress;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Lobby.LevelChoosing {
    public class LevelStartButton : MonoBehaviour {
        [SerializeField] Button playBtn;
        [SerializeField] RTLTextMeshPro title;

        [NonSerialized] public CoreLevelData coreLevelData;
        [NonSerialized] public LevelProgress.Level levelProgress;

        void Start() {
            playBtn.onClick.AddListener( onPlayButtonClick );
            playBtn.interactable = levelProgress.status.HasFlag( LevelProgress.LevelStatus.Unlocked );
            title.text = coreLevelData.title;
        }
        void onPlayButtonClick() {
            LobbyManager.Current.StartGame( coreLevelData, null );
        }
    }
}