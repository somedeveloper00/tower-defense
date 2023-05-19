using System;

namespace TowerDefense.Bridges.Analytics {
    public static class GameAnalyticsHelper {
        
        public enum Currency {
            Coin
        }
        
        public static string ToAnalyticString(this Currency currency) {
            return currency switch {
                Currency.Coin => "coin",
                _ => throw new ArgumentOutOfRangeException(nameof(currency), currency, null)
            };
        }
        
        public enum ItemType {
            Undefined, ItemType_IAP, ItemType_GameWin, ItemType_GameLose, ItemType_GameStart, ItemType_Ad, ItemType_Help
        }

        public static string ToAnalyticString(this ItemType itemType) {
            return itemType switch {
                ItemType.Undefined => "undefined",
                ItemType.ItemType_IAP => "iap",
                ItemType.ItemType_GameWin => "game-win",
                ItemType.ItemType_GameLose => "game-lose",
                ItemType.ItemType_GameStart => "game-start",
                ItemType.ItemType_Help => "help",
                ItemType.ItemType_Ad => "ad",
                _ => throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null)
            };
        }
    }
}