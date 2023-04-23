using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TowerDefense.Bridges.Iap {
    public abstract class InAppPurchase {
        public static InAppPurchase Current;

        protected InAppPurchase() => Current = this;

        public abstract Task<Result> InitializeIfNotAlready();
        
        public abstract void Close();

        public abstract Task<Result<List<PurchasableInfo>>> GetPurchasableInfos(string productId);
        
        public abstract Task<Result<List<SubscriptionInfo>>> GetSubscriptionInfos(string productId);
        
        public abstract Task<Result<List<PurchaseRecord>>> GetPurchaseRecords(string productId);
        
        public abstract Task<Result<List<PurchaseRecord>>> GetSubscriptionRecords(string productId);

        public abstract Task<Result<PurchaseRecord>> PurchasePurchasable(string productId, string payload = null);
        
        public abstract Task<Result<PurchaseRecord>> PurchaseSubscription(string productId, string payload = null);
        
        public abstract Task<Result> ConsumePurchase(string purchaseToken);

        [Serializable]
        public class PurchasableInfo {
            public string sku;
            public string title, description;
            public ulong price;
            public ulong rewardCoins;
            public ViewType viewType;
            public bool isAvailable = true;

            public enum ViewType {
                Small, Medium, Large
            }
        }

        [Serializable]
        public class SubscriptionInfo {
            public string sku, title, price, description;
            public DateTime subscriptionExpireDate;
        }

        [Serializable]
        public class PurchaseRecord {
            public enum State { Purchased = 0, Refunded = 1, Consumed = 2 }
            public string orderId, purchaseToken, payload, packageName, productId, originalJson, dataSignature;
            public State purchaseState;
            public long purchaseTime;
        }

        [Serializable]
        public class Result {
            public bool success;
            public static Result Failed => new Result() { success = false };
            public static Result Success => new Result() { success = true };
        }
        
        [Serializable]
        public class Result<T> : Result {
            public T data;
            public static Result<T> Failed => new Result<T>() { success = false };
            public static Result<T> Success => new Result<T>() { success = true };
        }
    }
}