using System.Collections.Generic;
using AnimFlex.Sequencer.UserEnd;
using DialogueSystem;
using TowerDefense.Common;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Lobby.ShopDialogue {
    [DeclareFoldoutGroup( "ref", Title = "References", Expanded = true )]
    public class ShopDialogue : Dialogue {

        [GroupNext("ref")]
        [SerializeField] List<PurchaseButton> purchaseButtons = new();
        [SerializeField] SequenceAnim outSeq;
        [SerializeField] Button closeBtn;
        
        [SerializeField, TextArea] string startLoadingTitleText;
        [SerializeField, TextArea] string startLoadingBodyText;
        [SerializeField, TextArea] string failedLoadingTitleText;
        [SerializeField, TextArea] string failedLoadingBodyText;
        [SerializeField] MessageDialogue.IconType failedLoadingIcon;

#if UNITY_EDITOR
        [Button]
        void fillPurchaseButtonsFromChildren() {
            purchaseButtons.Clear();
            purchaseButtons.AddRange( GetComponentsInChildren<PurchaseButton>( true ) );
        }
#endif


        protected override async void Start() {
            base.Start();
            closeBtn.onClick.AddListener( Close );
            
            var loading = DialogueManager.Current.GetOrCreate<MessageDialogue>( transform.parent );
            loading.UsePresetForLoading( startLoadingTitleText, startLoadingBodyText );
            loading.SetCloseButtonActive( true );
            loading.onClose += closeIfCanceledByUser;
            
            // load all purchasable items
            for (var i = 0; i < purchaseButtons.Count; i++) {
                var r = await purchaseButtons[i].updateInfo();
                if (!r) {
                    purchaseButtons.RemoveAt( i-- );
                }
            }
            // just in case if the tasks took too long
            if (this == null) return;

            loading.onClose -= closeIfCanceledByUser;

            // if no purchase item available, then quit
            if (purchaseButtons.Count == 0) {
                loading.SetLoadingLayoutActive( false );
                loading.SetTitleText( failedLoadingTitleText );
                loading.SetBodyText( failedLoadingBodyText );
                loading.SetIcon( failedLoadingIcon );
                loading.AddOkButton();
                await loading.AwaitClose();
                CloseWithAnim();
                return;
            }

            await loading.Close();

            void closeIfCanceledByUser() {
                Debug.Log( $"closing....result was {loading.result}" );
                if (loading.result == "cancel") {
                    CloseWithAnim();
                }
            }
        }

        public void CloseWithAnim() {
            Close();
            // outSeq.PlaySequence();
            // outSeq.sequence.onComplete += Close;
        }
    }
}