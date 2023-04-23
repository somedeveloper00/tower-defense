using System.Linq;
using RTLTMPro;
using TowerDefense.Background;
using TowerDefense.Bridges.Iap;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Lobby.ShopDialogue {
    public class PurchaseCard : MonoBehaviour {
        
        public InAppPurchase.PurchasableInfo productInfo;
            
        [SerializeField] RTLTextMeshPro titleTxt;
        [SerializeField] RTLTextMeshPro descriptionTxt;
        [SerializeField] RTLTextMeshPro costTxt;
        [SerializeField] RTLTextMeshPro skuTxt;
        [SerializeField] RTLTextMeshPro rewardCoinTxt;
        [SerializeField] Button purchaseBtn;
        [SerializeField] string costFormat;
        
        void Start() {
            purchaseBtn.onClick.AddListener( Purchase );
            UpdateView();
        }

        [Button]
        void UpdateView() {
            titleTxt.text = productInfo.title;
            skuTxt.text = productInfo.sku;
            descriptionTxt.text = productInfo.description;
            rewardCoinTxt.text = productInfo.rewardCoins.ToString( "#,0" ).En2PerNum();
            costTxt.text = costFormat.Replace( "$VAL", ( productInfo.price / 10 ).ToString( "#,0" ).En2PerNumV2() );
        }


        async void Purchase() {
            await GamePurchaseHandler.Current!.PurchaseProduct( productInfo );
        }
    }
}