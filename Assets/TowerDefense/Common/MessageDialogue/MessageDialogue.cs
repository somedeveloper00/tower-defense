using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnimFlex.Sequencer.UserEnd;
using DialogueSystem;
using RTLTMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Common {
    [DeclareFoldoutGroup("ref", Title = "References", Expanded = true)]
    public class MessageDialogue : Dialogue {
        [GroupNext("ref")]
        [SerializeField] Image panelImg;
        [SerializeField] Image titleBarImg;
        [SerializeField] GameObject loadingLayout;
        [SerializeField] GameObject closeLayout;
        [SerializeField] Button closeBtn;
        [SerializeField] Button outsideBtn;
        [SerializeField] RTLTextMeshPro bodyTxt;
        [SerializeField] GameObject bodyTxtLayout;
        [SerializeField] RTLTextMeshPro titleTxt;
        [SerializeField] GameObject titleTxtLayout;
        [SerializeField] SequenceAnim outSequence;
        [SerializeField] RectTransform buttonsLayout;
        [SerializeField] List<ButtonType> buttonTypes = new();
        [SerializeField] LoadingBar loadingBar;
        
        [UnGroupNext]
        [Title("Params")]
        [SerializeField] string[] loadingAdText;
        [SerializeField] string confirmTxt;
        [SerializeField] string cancelTxt;
        [SerializeField] string okTxt;
        
        /// <summary>
        /// the text of the clicked button. null if no button pressed, and "cancel" if closed by X or clicked outside dialogue
        /// </summary>
        [NonSerialized]
        public string result = null;
        
        Coroutine _bodyTxtCoroutine;

        protected override void Start() {
            base.Start();
            outsideBtn.onClick.AddListener( () => {
                result = "cancel";
                Close();
            } );
            closeBtn.onClick.AddListener( () => {
                result = "cancel";
                Close();
            } );
        }

        #region Options

        public void SetLoadingLayoutActive(bool active) => loadingLayout.SetActive( active );
        public void SetCloseButtonActive(bool active) => closeLayout.SetActive( active );
        public void SetCanCloseByOutsideClick(bool can) => outsideBtn.interactable = can;

        public void DisableBodyText() => bodyTxtLayout.SetActive( false );
        public void SetBodyText(string text) {
            bodyTxtLayout.SetActive( true );
            bodyTxt.text = text;
        }
        
        public void DisableTitleText() => titleTxtLayout.SetActive( false );
        public void SetTitleText(string text) {
            titleTxtLayout.SetActive( true );
            titleTxt.text = text;
        }
        
        public void SetBodyTextAnim(float delay, params string[] texts) {
            bodyTxtLayout.SetActive( true );
            if (_bodyTxtCoroutine != null)
                StopCoroutine( _bodyTxtCoroutine );
            _bodyTxtCoroutine = StartCoroutine( textAnim( delay, bodyTxt, texts ) );
        }

        IEnumerator textAnim(float delay, RTLTextMeshPro text, string[] values) {
            int indx = 0;
            while (true) {
                text.text = values[indx];
                yield return new WaitForSecondsRealtime( delay );
                indx = (indx + 1) % values.Length;
            }
        }

        public void AddButton(string buttonText, string buttonType) {
            var btn = Instantiate( buttonTypes.Find( b => b.name == buttonType ), buttonsLayout );
            btn.gameObject.SetActive( true );
            btn.text.text = buttonText;
            btn.button.onClick.AddListener( () => {
                result = buttonText;
                Close();
            } );
        }

        public void SetPanelColor(Color color) => panelImg.color = color;
        
        public void SetTitlebarColor(Color color) => titleBarImg.color = color;

        public void SetLoadingBarRotating() => loadingBar.PlayRotatingAnim();
        
        public void SetLoadingBarProgress(float progress) => loadingBar.SetProgress( progress );
#endregion


        public Task Close(bool noAnim = false) {
            if (noAnim) {
                base.Close();
            }
            else {
                canvasRaycaster.enabled = false;
                outSequence.PlaySequence();
                outSequence.sequence.onComplete += base.Close;
            }
            return AwaitClose();
        }

#region Helpers

        public void UsePresetForLoadingAd() {
            SetBodyTextAnim( 1, loadingAdText );
            DisableTitleText();
            SetLoadingLayoutActive( true );
            SetLoadingBarRotating();
            SetCloseButtonActive( false );
            SetCanCloseByOutsideClick( false );
        }

        /// <summary>
        /// result will be either "confirm" or "cancel"
        /// </summary>
        public void UsePresetForConfirmation(string questionText) {
            DisableTitleText();
            SetBodyText( questionText );
            AddButton( confirmTxt, "confirm" );
            SetCloseButtonActive( false );
            AddButton( cancelTxt, "cancel" );
        }

        public void UsePresetForNotice(string title, string noticeText) {
            SetTitleText( title );
            SetBodyText( noticeText );
            AddButton( okTxt, "ok" );
        }

#endregion


        
    }
}