using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bazaar.Data;
using Bazaar.Poolakey;
using Bazaar.Poolakey.Data;
using Newtonsoft.Json;
using RTLTMPro;
using UnityEngine;

namespace TowerDefense.Bridges.Iap {
    public class BazaarInAppPurchase : InAppPurchase {
        Payment payment;

        const string RSA =
            "MIHNMA0GCSqGSIb3DQEBAQUAA4G7ADCBtwKBrwC39wmdXPilKYXUAjVmrRlR/OjO3IGx+MxjPUThJw5KjeYDdGt54oBWexVEvKGQd4s0kW7oFspUa0N90gaQ+P1a/3EJzxISbnA73h/8AiYhKDWIhaEQsyOr9WvA75uvJd7gcpIUIwG0uxQi02o82zfyzQENzyBy88cDsEC4cSc6l4JMzDoKv7qfpQ0xUo03dDaFlVh2GYtPOJBx3tQAgKT861U8wTyDitMJngR8A/kCAwEAAQ==";

        const int RETRY_COUNT = 4;

        public override async Task<Result> InitializeIfNotAlready() {

            if (payment != null) return Result.Success;
            
            SecurityCheck securityCheck = SecurityCheck.Enable( RSA );
            PaymentConfiguration paymentConfiguration = new PaymentConfiguration( securityCheck );
            payment = new Payment( paymentConfiguration );

            int tryCount = 0;
            retry:
            tryCount++;

            var result = await payment.Connect();

            Debug.Log( $"msg: {result.message}, {result.data} , {result.stackTrace}" );
            Debug.Log( JsonConvert.SerializeObject( result ) );

            if (result.status != Status.Success) {
                if (tryCount < RETRY_COUNT) {
                    await Task.Delay( 1000 );
                    goto retry;
                }

                Debug.Log( "failed to connect to bazaar. max retries reached." );
                return Result.Failed;
            }

            return Result.Success;
        }

        public override void Close() {
            payment?.Disconnect();
        }

        public override async Task<Result<List<PurchasableInfo>>> GetPurchasableInfos(string productId) {

            await InitializeIfNotAlready();
            
            int tryCount = 0;
            retry:
            tryCount++;

            var result = await payment.GetSkuDetails( productId, SKUDetails.Type.inApp );
            if (result.status != Status.Success) {
                if (tryCount < RETRY_COUNT) {
                    Debug.Log( $"failed getting info for sku: {productId}. trying again ({tryCount})" );
                    await Task.Delay( 1000 );
                    goto retry;
                }

                Debug.Log( "failed to connect to bazaar. max retries reached." );
                return Result<List<PurchasableInfo>>.Failed;
            }

            Debug.Log( $"result: {JsonConvert.SerializeObject( result.data )}" );
            
            return new Result<List<PurchasableInfo>> {
                success = true,
                data = result.data.Select( data => new PurchasableInfo() {
                    sku = data.sku,
                    title = data.title,
                    price = ulong.Parse(Per2EnNum( data.price )),
                    description = data.description,
                    isAvailable = data.isAvailable
                } ).ToList()
            };
        }

        public override async Task<Result<List<SubscriptionInfo>>> GetSubscriptionInfos(string id) {

            await InitializeIfNotAlready();

            int tryCount = 0;
            retry:
            tryCount++;

            var result = await payment.GetSkuDetails( id, SKUDetails.Type.subscription );
            if (result.status != Status.Success) {
                if (tryCount < RETRY_COUNT) {
                    await Task.Delay( 1000 );
                    goto retry;
                }

                Debug.Log( "failed to connect to bazaar. max retries reached." );
                return Result<List<SubscriptionInfo>>.Failed;
            }
            
            Debug.Log( $"result: {JsonConvert.SerializeObject( result.data )}" );

            return new() {
                success = true,
                data = result.data.Select( data => new SubscriptionInfo() {
                    sku = data.sku,
                    title = data.title,
                    price = data.title,
                    description = data.description,
                    subscriptionExpireDate = data.subscriptionExpireDate
                } ).ToList()
            };
        }

        public override async Task<Result<List<PurchaseRecord>>> GetPurchaseRecords(string id) {

            await InitializeIfNotAlready();

            int tryCount = 0;
            retry:
            tryCount++;

            var result = await payment.GetPurchases( SKUDetails.Type.inApp );
            if (result.status != Status.Success) {
                if (tryCount < RETRY_COUNT) {
                    await Task.Delay( 1000 );
                    goto retry;
                }

                Debug.Log( "failed to connect to bazaar. max retries reached." );
                return Result<List<PurchaseRecord>>.Failed;
            }
            
            Debug.Log( $"result: {JsonConvert.SerializeObject( result.data )}" );

            return new() {
                success = true,
                data = result.data.Select( data => new PurchaseRecord() {
                    purchaseState = (PurchaseRecord.State)data.purchaseState,
                    payload = data.payload,
                    dataSignature = data.dataSignature,
                    orderId = data.orderId,
                    productId = data.productId,
                    originalJson = data.originalJson,
                    packageName = data.packageName,
                    purchaseTime = data.purchaseTime,
                    purchaseToken = data.purchaseToken,
                } ).ToList()
            };
        }

        public override async Task<Result<List<PurchaseRecord>>> GetSubscriptionRecords(string id) {

            await InitializeIfNotAlready();

            int tryCount = 0;
            retry:
            tryCount++;

            var result = await payment.GetPurchases( SKUDetails.Type.subscription );
            if (result.status != Status.Success) {
                if (tryCount < RETRY_COUNT) {
                    await Task.Delay( 1000 );
                    goto retry;
                }

                Debug.Log( "failed to connect to bazaar. max retries reached." );
                return Result<List<PurchaseRecord>>.Failed;
            }

            Debug.Log( $"result: {JsonConvert.SerializeObject( result.data )}" );


            return new() {
                success = true,
                data = result.data.Select( data => new PurchaseRecord() {
                    purchaseState = (PurchaseRecord.State)data.purchaseState,
                    payload = data.payload,
                    dataSignature = data.dataSignature,
                    orderId = data.orderId,
                    productId = data.productId,
                    originalJson = data.originalJson,
                    packageName = data.packageName,
                    purchaseTime = data.purchaseTime,
                    purchaseToken = data.purchaseToken,
                } ).ToList()
            };
        }

        public override async Task<Result<PurchaseRecord>> PurchasePurchasable(string productId, string payload = null) {

            await InitializeIfNotAlready();

            int tryCount = 0;
            retry:
            tryCount++;

            var result = await payment.Purchase( productId, SKUDetails.Type.inApp, payload: payload );
            if (result.status == Status.Unknown) {
                if (tryCount < RETRY_COUNT) {
                    await Task.Delay( 1000 );
                    goto retry;
                }

                Debug.Log( "failed to connect to bazaar. max retries reached." );
                return Result<PurchaseRecord>.Failed;
            }

            if (result.status != Status.Success) {
                return Result<PurchaseRecord>.Failed;
            }
            
            Debug.Log( $"result: {JsonConvert.SerializeObject( result.data )}" );

            return new() {
                success = true,
                data = new PurchaseRecord() {
                    purchaseState = (PurchaseRecord.State)result.data.purchaseState,
                    payload = result.data.payload,
                    dataSignature = result.data.dataSignature,
                    orderId = result.data.orderId,
                    productId = result.data.productId,
                    originalJson = result.data.originalJson,
                    packageName = result.data.packageName,
                    purchaseTime = result.data.purchaseTime,
                    purchaseToken = result.data.purchaseToken,
                }
            };
        }

        public override async Task<Result<PurchaseRecord>> PurchaseSubscription(string productId, string payload = null) {

            await InitializeIfNotAlready();

            int tryCount = 0;
            retry:
            tryCount++;

            var result = await payment.Purchase( productId, SKUDetails.Type.subscription, payload: payload );
            if (result.status != Status.Success) {
                if (tryCount < RETRY_COUNT) {
                    await Task.Delay( 1000 );
                    goto retry;
                }

                Debug.Log( "failed to connect to bazaar. max retries reached." );
                return Result<PurchaseRecord>.Failed;
            }
            
            Debug.Log( $"result: {JsonConvert.SerializeObject( result.data )}" );

            return new() {
                success = true,
                data = new PurchaseRecord() {
                    purchaseState = (PurchaseRecord.State)result.data.purchaseState,
                    payload = result.data.payload,
                    dataSignature = result.data.dataSignature,
                    orderId = result.data.orderId,
                    productId = result.data.productId,
                    originalJson = result.data.originalJson,
                    packageName = result.data.packageName,
                    purchaseTime = result.data.purchaseTime,
                    purchaseToken = result.data.purchaseToken,
                }
            };
        }

        public override async Task<Result> ConsumePurchase(string purchaseToken) {

            await InitializeIfNotAlready();

            int tryCount = 0;
            retry:
            tryCount++;

            var result = await payment.Consume( purchaseToken );
            if (result.status != Status.Success) {
                if (tryCount < RETRY_COUNT) {
                    await Task.Delay( 1000 );
                    goto retry;
                }

                Debug.Log( "failed to connect to bazaar. max retries reached." );
                return Result.Failed;
            }
            
            Debug.Log( $"result: {JsonConvert.SerializeObject( result.data )}" );

            return Result.Success;
        }

        public static string Per2EnNum(string perNum) {
            var builder = new StringBuilder( perNum );
            for (int i = 0; i < builder.Length; i++) {
                switch (builder[i]) {
                    case '۰': builder[i] = '0'; break;
                    case '۱': builder[i] = '1'; break;
                    case '۲': builder[i] = '2'; break;
                    case '۳': builder[i] = '3'; break;
                    case '۴': builder[i] = '4'; break;
                    case '۵': builder[i] = '5'; break;
                    case '۶': builder[i] = '6'; break;
                    case '۷': builder[i] = '7'; break;
                    case '۸': builder[i] = '8'; break;
                    case '۹': builder[i] = '9'; break;
                    default: builder.Remove( i--, 1 ); break;
                }
            }

            return builder.Length == 0 ? "0" : builder.ToString();
        }
    }
}