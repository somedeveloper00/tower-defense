using System;
using DialogueSystem;
using RTLTMPro;
using TowerDefense.Bridges.Iap;
using TowerDefense.Common;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TowerDefense.Lobby.ShopDialogue {
    public class PurchaseButton : MonoBehaviour {
        [FormerlySerializedAs( "productId" )] public string sku;
        public string title;
        public string description;
        public ulong cost;
            
        [SerializeField] RTLTextMeshPro titleTxt;
        [SerializeField] RTLTextMeshPro descriptionTxt;
        [SerializeField] RTLTextMeshPro costTxt;
        [SerializeField] Button purchaseBtn;

        void Start() {
            purchaseBtn.onClick.AddListener( async () => {
                var loading = DialogueManager.Current.GetOrCreate<MessageDialogue>();
                loading.UsePresetForLoading();
                var result = await InAppPurchase.Current.PurchasePurchasable( sku );
                if (result.success)
                    await InAppPurchase.Current.ConsumePurchase( result.data.purchaseToken );
                await loading.Close();
            } );
        }

        public void UpdateView() {
            titleTxt.text = title;
            descriptionTxt.text = description;
            costTxt.text = cost.ToString( "#,0" );
        }
    }
}