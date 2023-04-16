﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bazaar.Data;
using Bazaar.Poolakey;
using Bazaar.Poolakey.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace TowerDefense.Bridges.Iap {
    public class BazaarInAppPurchase : InAppPurchase {
        Payment payment;

        const string RSA =
            "MIHNMA0GCSqGSIb3DQEBAQUAA4G7ADCBtwKBrwC39wmdXPilKYXUAjVmrRlR/OjO3IGx+MxjPUThJw5KjeYDdGt54oBWexVEvKGQd4s0kW7oFspUa0N90gaQ+P1a/3EJzxISbnA73h/8AiYhKDWIhaEQsyOr9WvA75uvJd7gcpIUIwG0uxQi02o82zfyzQENzyBy88cDsEC4cSc6l4JMzDoKv7qfpQ0xUo03dDaFlVh2GYtPOJBx3tQAgKT861U8wTyDitMJngR8A/kCAwEAAQ==";

        const int RETRY_COUNT = 4;

        public override async Task<Result> Initialize() {

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

        public override async Task<Result<List<PurchasableInfo>>> GetPurchasableInfos(string id) {

            int tryCount = 0;
            retry:
            tryCount++;

            var result = await payment.GetSkuDetails( id, SKUDetails.Type.inApp );
            if (result.status != Status.Success) {
                if (tryCount < RETRY_COUNT) {
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
                    price = data.title,
                    description = data.description,
                    isAvailable = data.isAvailable
                } ).ToList()
            };
        }

        public override async Task<Result<List<SubscriptionInfo>>> GetSubscriptionInfos(string id) {

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

        public override async Task<Result<PurchaseRecord>>
            PurchasePurchasable(string productId, string payload = null) {

            int tryCount = 0;
            retry:
            tryCount++;

            var result = await payment.Purchase( productId, SKUDetails.Type.inApp, payload: payload );
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

        public override async Task<Result<PurchaseRecord>>
            PurchaseSubscription(string productId, string payload = null) {

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
    }
}