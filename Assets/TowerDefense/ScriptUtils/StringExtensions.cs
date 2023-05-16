using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TowerDefense
{
    public static class StringExtensions
    {
        public static string SetHtmlColor(this string value, Color color) {
            return $"<color=#{ColorUtility.ToHtmlStringRGB( color )}>{value}</color>";
        }
        
        /// <summary>
        /// ۱,۲۳۴۴ => 1,2344
        /// </summary>
        public static string Per2EnNum(this string perNum, bool keepCommas = true) {
            var builder = new StringBuilder( perNum );
            char lastChar = ' ';
            for (int i = 0; i < builder.Length; i++) {
                bool removed = false;
                switch (builder[i]) {
                    case '۰' or '0': builder[i] = '0'; break;
                    case '۱' or '1': builder[i] = '1'; break;
                    case '۲' or '2': builder[i] = '2'; break;
                    case '۳' or '3': builder[i] = '3'; break;
                    case '۴' or '4': builder[i] = '4'; break;
                    case '۵' or '5': builder[i] = '5'; break;
                    case '۶' or '6': builder[i] = '6'; break;
                    case '۷' or '7': builder[i] = '7'; break;
                    case '۸' or '8': builder[i] = '8'; break;
                    case '۹' or '9': builder[i] = '9'; break;
                    
                    // accept one comma in a row
                    case ',' when keepCommas && lastChar != ',': break;
                    
                    default: 
                        builder.Remove( i--, 1 );
                        removed = true; break;
                }
                if (!removed) lastChar = builder[i];
            }

            return builder.Length == 0 ? "0" : builder.ToString();
        }

        /// <summary>
        ///  abc1,2344 => ۱,۲۳۴۴
        /// </summary>
        public static string En2PerNum(this string enNum, bool keepCommas = true) {
            var builder = new StringBuilder( enNum );
            char lastChar = ' ';
            for (int i = 0; i < builder.Length; i++) {
                bool removed = false;
                switch (builder[i]) {
                    case '۰' or '0': builder[i] = '۰'; break;
                    case '۱' or '1': builder[i] = '۱'; break;
                    case '۲' or '2': builder[i] = '۲'; break;
                    case '۳' or '3': builder[i] = '۳'; break;
                    case '۴' or '4': builder[i] = '۴'; break;
                    case '۵' or '5': builder[i] = '۵'; break;
                    case '۶' or '6': builder[i] = '۶'; break;
                    case '۷' or '7': builder[i] = '۷'; break;
                    case '۸' or '8': builder[i] = '۸'; break;
                    case '۹' or '9': builder[i] = '۹'; break;
                    
                    // accept one comma in a row
                    case ',' when keepCommas && lastChar != ',': break;
                    
                    default: 
                        builder.Remove( i--, 1 );
                        removed = true; break;
                }
                if (!removed) lastChar = builder[i];
            }

            return builder.Length == 0 ? "۰" : builder.ToString();
        }

        
        /// <summary>
        /// 1,2334 => ۴۳۳۲,۱
        /// </summary>
        public static string En2PerNumV2(this string enNum, bool keepCommas = true) {
            return enNum.En2PerNum().Reverse();
        }

        public static string Reverse(this string s) {
            char[] charArray = s.ToCharArray();
            Array.Reverse( charArray );
            return new string( charArray );
        }

    }
}