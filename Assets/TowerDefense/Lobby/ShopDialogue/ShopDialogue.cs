using System.Collections.Generic;
using AnimFlex.Sequencer.UserEnd;
using DialogueSystem;
using TowerDefense.Bridges.Iap;
using TowerDefense.Common;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Lobby.ShopDialogue {
    public class ShopDialogue : Dialogue {

        public string productId;

        [SerializeField] SequenceAnim outSeq;
        [SerializeField] string startLoadingTitleText;
        [SerializeField] string startLoadingBodyText;
        [SerializeField] string failedLoadingTitleText;
        [SerializeField] string failedLoadingBodyText;
        [SerializeField] MessageDialogue.IconType failedLoadingIcon;
        [SerializeField] PurchaseButton purchaseButtonSample;
        [SerializeField] Button closeBtn;

        List<PurchaseButton> purchaseButtons = new();

        protected override async void Start() {
            base.Start();
            closeBtn.onClick.AddListener( Close );
            
            var loading = DialogueManager.Current.GetOrCreate<MessageDialogue>( transform.parent );
            loading.UsePresetForLoading( startLoadingTitleText, startLoadingBodyText );
            var result = await InAppPurchase.Current.GetPurchasableInfos( productId );
            
            // failed. close this dialogue and ask to come back later
            if (!result.success) {
                loading.SetLoadingLayoutActive( false );
                loading.SetTitleText( failedLoadingTitleText );
                loading.SetBodyText( failedLoadingBodyText );
                loading.SetIcon( failedLoadingIcon );
                loading.AddOkButton();
                await loading.AwaitClose();
                CloseWithAnim();
            }
            else {
                setUpPurchaseButtons( result.data );
            }
        }

        void setUpPurchaseButtons(List<InAppPurchase.PurchasableInfo> purchasableInfos) {
            foreach (var purchaseInfo in purchasableInfos) {
                var purchaseButton = Instantiate( purchaseButtonSample, purchaseButtonSample.transform.parent );
                purchaseButton.sku = purchaseInfo.sku;
                purchaseButton.cost = ulong.Parse( purchaseInfo.price );
                purchaseButton.title = purchaseInfo.title;
                purchaseButton.description = purchaseInfo.description;
                purchaseButton.UpdateView();
                purchaseButtons.Add( purchaseButton );
            }
        }

        public void CloseWithAnim() {
            outSeq.PlaySequence();
            outSeq.sequence.onComplete += Close;
        }
    }
}