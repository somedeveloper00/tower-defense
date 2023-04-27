using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DialogueSystem;
using GameAnalyticsSDK;
using TowerDefense.Bridges.Analytics;
using TowerDefense.Bridges.Iap;
using TowerDefense.Common;
using TowerDefense.Data;
using UnityEngine;

namespace TowerDefense.Background {
    /// <summary>
    /// Handles the in app purchase of products for veiw and models
    /// </summary>
    public class GamePurchaseHandler : MonoBehaviour {

        public static GamePurchaseHandler Current;

        void OnEnable() => Current = this;

        public List<InAppPurchase.PurchasableInfo> AllPurchasableInfos;

        public List<InAppPurchase.PurchasableInfo> AvailablePurchsableInfos =>
            AllPurchasableInfos.Where( p => p.isAvailable ).ToList();

        [SerializeField] string updatingDataTitleTxt;
        [SerializeField, TextArea] string updatingDataBodyTxt;
        [SerializeField] string purchasingTitleTxt;
        [SerializeField, TextArea] string purchasingBodyTxt;
        [SerializeField] string unsuccessfulPurchaseTitleTxt;
        [SerializeField, TextArea] string unsuccessfulPurchaseBodyTxt;
        [SerializeField] MessageDialogue.IconType unsuccessfulPurchaseIcon;

        public async Task<bool> UpdateData() {
            var loading = DialogueManager.Current.GetOrCreate<MessageDialogue>();
            loading.UsePresetForLoading( updatingDataTitleTxt, updatingDataBodyTxt );
            var result = await InAppPurchase.Current.GetPurchasableInfos( string.Join( ",", AllPurchasableInfos.Select( p => p.sku ) ) );
            if (!result.success) {
                loading.SetLoadingLayoutActive( false );
                loading.SetTitleText( unsuccessfulPurchaseTitleTxt );
                loading.SetBodyText( unsuccessfulPurchaseBodyTxt );
                loading.SetIcon( unsuccessfulPurchaseIcon );
                loading.SetCloseButtonActive( true );
                loading.SetCanCloseByOutsideClick( true );
                loading.AddOkButton();
                await loading.AwaitClose();
            }
            else {
                AllPurchasableInfos.Clear();
                for (int i = 0; i < result.data.Count; i++) {
                    if (result.data[i] != null) AllPurchasableInfos.Add( result.data[i] );
                }
                await loading.Close();
            }

            return result.success;
        }
        
        public async Task<bool> PurchaseProduct(InAppPurchase.PurchasableInfo product) {
            var loading = DialogueManager.Current.GetOrCreate<MessageDialogue>();
            loading.UsePresetForLoading( purchasingTitleTxt, purchasingBodyTxt );
            var result = await InAppPurchase.Current.PurchasePurchasable( product.sku );
            if (!result.success) {
                loading.SetLoadingLayoutActive( false );
                loading.SetTitleText( unsuccessfulPurchaseTitleTxt );
                loading.SetBodyText( unsuccessfulPurchaseBodyTxt );
                loading.SetIcon( unsuccessfulPurchaseIcon );
                loading.AddOkButton();
                loading.SetCanCloseByOutsideClick( true );
                loading.SetCloseButtonActive( true );
                await loading.AwaitClose();
                return false;
            }
            else {
                var consumeResult = await InAppPurchase.Current.ConsumePurchase( result.data.purchaseToken );
                if (!consumeResult.success) {
                    loading.SetLoadingLayoutActive( false );
                    loading.SetTitleText( unsuccessfulPurchaseTitleTxt );
                    loading.SetBodyText( unsuccessfulPurchaseBodyTxt );
                    loading.SetIcon( unsuccessfulPurchaseIcon );
                    await loading.AwaitClose();
                    return false;
                }
            }
            // SUCCESS
            PlayerGlobals.Current.ecoProg.coins += product.rewardCoins;
            
            GameAnalytics.NewResourceEvent( GAResourceFlowType.Source, GameAnalyticsHelper.Currency_Coin,
                product.rewardCoins, GameAnalyticsHelper.ItemType_IAP, "InAppPurchase" );
            
            await loading.Close();
            return true;
        }
    }
}