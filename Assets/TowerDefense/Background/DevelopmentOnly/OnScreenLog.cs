using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.DevelopmentOnly {
    [RequireComponent(typeof(Text))]
    public class OnScreenLog : MonoBehaviour {
        Text txt;
        static string log;
        public static void Log(string message) => log += message + "\n";
        void Start() => txt = GetComponent<Text>();
        void Update() => txt.text = log;
    }
}