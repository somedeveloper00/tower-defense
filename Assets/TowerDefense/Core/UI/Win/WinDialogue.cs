using System.Threading.Tasks;
using DialogueSystem;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Core.UI.Win {
    [DeclareFoldoutGroup("ref", Title = "References", Expanded = true)]
    public class WinDialogue : Dialogue {
        public WinData winData;

        [GroupNext( "ref" )]
        [SerializeField] GameObject[] starObjects;
        [SerializeField] Button returnButton;

        protected override void Start() {
            base.Start();
            canvasGroup.alpha = 0;
            for (int i = 0; i < starObjects.Length; i++) {
                starObjects[i].SetActive( i < winData.stars );
            }
            returnButton.onClick.AddListener( onLobbyBtnClick );
        }

        async void onLobbyBtnClick() {
            await Task.Delay( 1000 );
            canvasRaycaster.enabled = false;
            CoreGameManager.Current.BackToLobby();
            Close();
        }
    }
}