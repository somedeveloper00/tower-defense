using System;
using System.Collections.Generic;
using AnimFlex.Sequencer.UserEnd;
using DialogueSystem;
using TowerDefense.Background;
using TowerDefense.Bridges.Iap;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Lobby.ShopDialogue {
    [DeclareFoldoutGroup( "ref", Title = "References", Expanded = true )]
    public class ShopDialogue : Dialogue {

        [GroupNext( "ref" )] 
        [SerializeField] Transform purchasableProductParent;
        [SerializeField] List<PurchasableProductSample> purchasableProductSamples;
        [SerializeField] SequenceAnim inSeq, outSeq;
        [SerializeField] Button closeBtn;
        
        [Serializable]
        class PurchasableProductSample {
            public PurchaseCard prefab;
            public InAppPurchase.PurchasableInfo.ViewType viewType;
        }
        

        bool _opened = false;
        List<PurchaseCard> purchaseCards = new();
        
        protected override async void Start() {
            base.Start();
            canvasGroup.alpha = 0;
            canvasRaycaster.enabled = false;
            
            var success = await GamePurchaseHandler.Current.UpdateData();
            if (!success || GamePurchaseHandler.Current.AvailablePurchsableInfos.Count == 0) {
                Close();
                return;
            }

            // SUCCESS

            // just in case if the tasks took too long
            if (this == null) return;

            var productInfos = GamePurchaseHandler.Current.AvailablePurchsableInfos;
            for (int i = 0; i < productInfos.Count; i++) {
                var purchaseCard = Instantiate(
                    purchasableProductSamples.Find( p => p.viewType == productInfos[i].viewType ).prefab,
                    purchasableProductParent );
                purchaseCard.productInfo = productInfos[i];
                purchaseCard.gameObject.SetActive( true );
                purchaseCards.Add( purchaseCard );
            }
            
            // show dialogue
            closeBtn.onClick.AddListener( CloseWithAnim );
            inSeq.PlaySequence();
            inSeq.sequence.onComplete += () => {
                canvasRaycaster.enabled = true;
                _opened = true;
            };
        }

        public void CloseWithAnim() {
            if (_opened) {
                outSeq.PlaySequence();
                outSeq.sequence.onComplete += Close;
            }
            else {
                Close();
            }
        }
    }
}