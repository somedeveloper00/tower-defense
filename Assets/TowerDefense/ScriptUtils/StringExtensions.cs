using UnityEngine;

namespace TowerDefense.ScriptUtils
{
    public static class StringExtensions
    {
        public static string SetHtmlColor(this string value, Color color) {
            return $"<color=#{ColorUtility.ToHtmlStringRGB( color )}>{value}</color>";
        }
    }
}