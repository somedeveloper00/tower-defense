using System;
using AnimFlex.Sequencer.UserEnd;
using DialogueSystem;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;
using Sequence = AnimFlex.Sequencer.Sequence;

namespace TowerDefense.Core.UI.Win {
    [DeclareFoldoutGroup("ref", Title = "References")]
    public class WinDialogue : Dialogue {
        public int stars = 0;

        [GroupNext( "ref" )]
        [SerializeField] SequenceAnim inSequence, outSequence;
        [SerializeField] GameObject[] starObjects;
        [SerializeField] Button okButton;

        protected override void OnEnable() {
            base.OnEnable();
        }


        protected override void Start() {
            base.Start();
            for (int i = 0; i < starObjects.Length; i++) {
                starObjects[i].SetActive( i < stars );
            }
            okButton.onClick.AddListener( onOkButtonClick );
            inSequence.PlaySequence();
        }

        void onOkButtonClick() {
            CloseWithAnim();
        }

        public void CloseWithAnim() {
            outSequence.PlaySequence();
            outSequence.sequence.onComplete += this.Close;
        }
    }
}