using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnimFlex.Sequencer;
using DialogueSystem;
using RTLTMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.UI {
    [DeclareFoldoutGroup("ref", Title = "References", Expanded = true)]
    public class MessageDialogue : Dialogue {
        
        [GroupNext("ref")]
        [SerializeField] Image panelImg;
        [SerializeField] Image titleBarImg;
        [SerializeField] GameObject loadingLayout;
        [SerializeField] GameObject closeLayout;
        [SerializeField] GameObject iconLayout;
        [SerializeField] Image iconImage;
        [SerializeField] DelayedButton closeBtn;
        [SerializeField] DelayedButton outsideBtn;
        [SerializeField] RTLTextMeshPro bodyTxt;
        [SerializeField] GameObject bodyTxtLayout;
        [SerializeField] RTLTextMeshPro titleTxt;
        [SerializeField] GameObject titleTxtLayout;
        [SerializeField] SequenceAnim inSequence, outSequence;
        [SerializeField] RectTransform buttonsLayout;
        [SerializeField] List<TextButton> buttonTypes = new();
        [SerializeField] LoadingBar loadingBar;

        [UnGroupNext] [Title( "Params" )] 
        [SerializeField] float exitDelay = 0.3f;
        [SerializeField] string[] loadingAdText;
        [SerializeField] string confirmTxt;
        [SerializeField] string cancelTxt;
        [SerializeField] string okTxt;
        [SerializeField] string adCancelByUserTitle;
        [SerializeField, TextArea] string adCancelByUserBody;
        [SerializeField] string adFailedTitle;
        [SerializeField, TextArea] string adFailedBody;
        [SerializeField] Icon[] icons;

        [Serializable]
        public class Icon {
            public IconType type;
            public Texture2D texture;
        }
        public enum IconType {
            None,
            MonkeySad, MonkeyThinking, MonkeyThumbsUp, 
            ShooterThumbsUp, ShooterAngry
        }
        
        /// <summary>
        /// the text of the clicked button. null if no button pressed, and "cancel" if closed by X or clicked outside dialogue
        /// </summary>
        [NonSerialized]
        public string result = null;
        
        Coroutine _bodyTxtCoroutine;
        bool _opened = false;

        protected override void Start() {
            base.Start();
            // canvasGroup.alpha = 0; // for inSeq anim
            outsideBtn.onClick.AddListener( () => {
                result = "cancel";
                Close();
            } );
            closeBtn.onClick.AddListener( () => {
                result = "cancel";
                Close();
            } );
            inSequence.PlaySequence();
            inSequence.sequence.onComplete += () => _opened = true;
        }

#region Options

        public void SetLoadingLayoutActive(bool active) => loadingLayout.SetActive( active );
        
        public void SetCloseButtonActive(bool active) => closeLayout.SetActive( active );
        
        public void SetCanCloseByOutsideClick(bool can) => outsideBtn.interactable = can;

        public void DisableBodyText() => bodyTxtLayout.SetActive( false );
       
        public void SetBodyText(string text) {
            // stop text anim if playing
            if (_bodyTxtCoroutine != null)
                StopCoroutine( _bodyTxtCoroutine );
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
                canvasRaycaster.enabled = false;
                result = buttonText;
            #pragma warning disable CS4014
                Close();
            #pragma warning restore CS4014
            } );
        }

        public void SetPanelColor(Color color) => panelImg.color = color;
        
        public void SetTitlebarColor(Color color) => titleBarImg.color = color;

        public void SetLoadingBarRotating() => loadingBar.PlayRotatingAnim();
        
        public void SetLoadingBarProgress(float progress) => loadingBar.SetProgress( progress );

        public void SetIcon(IconType iconType) {
            if (iconType == IconType.None) {
                iconLayout.SetActive( false );
                return;
            }
            iconLayout.SetActive( true );
            var tex = icons.First( i => i.type == iconType ).texture;
            iconImage.sprite = Sprite.Create( tex, new Rect( 0, 0, tex.width, tex.height ), Vector2.zero );
        }
#endregion


        public async Task Close(bool noAnim = false) {
            canvasRaycaster.enabled = false;
            await Task.Delay( (int)(exitDelay * 1000) );
            if (noAnim) {
                if (inSequence.sequence.IsPlaying())
                    inSequence.sequence?.Stop();
                base.Close();
            }
            else {
                while (!_opened) await Task.Yield();
                canvasRaycaster.enabled = false;
                outSequence.PlaySequence();
                await outSequence.AwaitComplete();
                base.Close();
            }
            await AwaitClose();
        }

#region Helpers

        /// <summary>
        /// general loading dialogue. the body text will ahve three dots animation
        /// </summary>
        public void UsePresetForLoading(string titleText = null, string bodyText = null) {
            if (!string.IsNullOrEmpty( bodyText )) {
                SetBodyTextAnim( 1, bodyText, bodyText + ".", bodyText + "..", bodyText + "..." );
            }
            else {
                DisableBodyText();
            }
            if (!string.IsNullOrEmpty( titleText )) {
                SetTitleText( titleText );
            }
            else {
                DisableTitleText();
            }
            SetLoadingLayoutActive( true );
            SetLoadingBarRotating();
            SetCloseButtonActive( false );
            SetCanCloseByOutsideClick( false );
        }

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
            AddConfirmButton();
            SetCloseButtonActive( false );
            AddCancelButton();
        }

        public void UsePresetForAdCancelledByUser() {
            SetTitleText( adCancelByUserTitle );
            SetBodyText( adCancelByUserBody );
            SetIcon( IconType.MonkeySad );
            AddOkButton();
        }

        public void UsePresetForAdFailed() {
            SetTitleText( adFailedTitle );
            SetBodyText( adFailedBody );
            SetIcon( IconType.MonkeyThinking );
            AddOkButton();
        }

        public void UsePresetForNotice(string title, string noticeText, IconType icon = IconType.None) {
            SetTitleText( title );
            SetBodyText( noticeText );
            AddOkButton();
        }

        public void AddOkButton() {
            AddButton( okTxt, "ok" );
        }

        public void AddConfirmButton() {
            AddButton( confirmTxt, "confirm" );
        }

        public void AddCancelButton() {
            AddButton( cancelTxt, "cancel" );
        }

#endregion
    }
}