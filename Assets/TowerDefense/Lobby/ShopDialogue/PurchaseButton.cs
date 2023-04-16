using System.Threading.Tasks;
using DialogueSystem;
using RTLTMPro;
using TowerDefense.Bridges.Iap;
using TowerDefense.Common;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Lobby.ShopDialogue {
    public class PurchaseButton : MonoBehaviour {
        
        public InAppPurchase.PurchasableInfo productInfo;
            
        [SerializeField] RTLTextMeshPro titleTxt;
        [SerializeField] RTLTextMeshPro descriptionTxt;
        [SerializeField] RTLTextMeshPro costTxt;
        [SerializeField] RTLTextMeshPro skuTxt;
        [SerializeField] Button purchaseBtn;
        [SerializeField] string costFormat;

        public async Task<bool> updateInfo() {
            Debug.Log( $"updating info for sku: {productInfo.sku}" );
            var result = await InAppPurchase.Current.GetPurchasableInfos( productInfo.sku );
            if (!result.success || !result.data[0].isAvailable) {
                Destroy( gameObject );
                return false;
            }

            productInfo = result.data[0];
            UpdateView();
            return true;
        }

        void Start() {
            purchaseBtn.onClick.AddListener( async () => {
                var loading = DialogueManager.Current.GetOrCreate<MessageDialogue>();
                loading.UsePresetForLoading();
                var result = await InAppPurchase.Current.PurchasePurchasable( productInfo.sku );
                Debug.Log( $"purchase successful" );
                if (result.success) {
                    var consumeResult = await InAppPurchase.Current.ConsumePurchase( result.data.purchaseToken );
                    if (consumeResult.success) {
                        Debug.Log( $"consume successful" );
                    }
                }

                await loading.Close();
            } );
        }

        [Button]
        void UpdateView() {
            titleTxt.text = productInfo.title;
            skuTxt.text = productInfo.sku;
            descriptionTxt.text = productInfo.description;
            
            var strBuilder = new FastStringBuilder( ( productInfo.price / 10 ).ToString( "#,0" ) );
            RTLTMPro.GlyphFixer.FixNumbers( strBuilder, true );
            strBuilder.Reverse();
            costTxt.text = costFormat.Replace( "$VAL$", strBuilder.ToString() );
        }
    }
}