using System;
using AnimFlex.Sequencer.UserEnd;
using DialogueSystem;
using TriInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Core.UI.Lose {
    [DeclareFoldoutGroup("ref", Title = "References", Expanded = true)]
    public class LoseDialogue : Dialogue {

        public Action onRetry;
        public Action onRetrunHome;
        
        [GroupNext("ref")]
        [SerializeField] SequenceAnim inSequence, outSequence;
        [SerializeField] Button retryBtn, returnBtn;

        protected override void OnEnable() {
            base.OnEnable();
            
            canvasRaycaster.enabled = false;
            canvasGroup.alpha = 0;
            
            retryBtn.onClick.AddListener( onRetryBtnClick );
            returnBtn.onClick.AddListener( onReturnBtnClick );
        }

        protected override void Start() {
            base.Start();
            inSequence.PlaySequence();
            inSequence.sequence.onComplete -= enableListeners;
            inSequence.sequence.onComplete += enableListeners;
        }
        
        void enableListeners() {
            canvasRaycaster.enabled = true;
        }
        
        void onRetryBtnClick() {
            onRetry?.Invoke();
        }
        
        void onReturnBtnClick() {
            onRetrunHome?.Invoke();
            outSequence.PlaySequence();
        }
    }
}