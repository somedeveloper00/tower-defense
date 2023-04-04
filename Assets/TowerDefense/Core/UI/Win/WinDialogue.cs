using AnimFlex.Sequencer.UserEnd;
using DialogueSystem;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Core.UI.Win {
    [DeclareFoldoutGroup("ref", Title = "References", Expanded = true)]
    public class WinDialogue : Dialogue {
        public int stars = 0;

        [GroupNext( "ref" )]
        [SerializeField] SequenceAnim inSequence, outSequence;
        [SerializeField] GameObject[] starObjects;
        [SerializeField] Button returnButton;

        protected override void OnEnable() {
            base.OnEnable();
            canvasGroup.alpha = 0;
        }

        protected override void Start() {
            base.Start();
            for (int i = 0; i < starObjects.Length; i++) {
                starObjects[i].SetActive( i < stars );
            }
            returnButton.onClick.AddListener( onLobbyBtnClick );
            inSequence.PlaySequence();
        }

        void onLobbyBtnClick() {
            canvasRaycaster.enabled = false;
            CoreGameManager.Current.BackToLobby();
            Close();
        }
    }
}