using System;
using System.Threading.Tasks;
using DialogueSystem;
using RTLTMPro;
using TowerDefense.Common;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Lobby {
    public class Tests : MonoBehaviour {
        public RTLTextMeshPro debugtxt;
        public Button start, stop, fullTest;

#if UNITY_EDITOR
        [Button]
        async void MessageTestAd() {
            var dialogue = DialogueManager.Current.GetOrCreate<MessageDialogue>();
            dialogue.UsePresetForLoadingAd();
            await Task.Delay( 5000 );
            await dialogue.Close();
        }

        [Button]
        void MessageTestNotice() {
            var dialogue = DialogueManager.Current.GetOrCreate<MessageDialogue>();
            dialogue.UsePresetForNotice( "اوهوی! با توام!!!", "این یک تست برای خبررسانی است. \n حالت خوبه؟" );
        }

        [Button]
        void MessageTestConf() {
            var dialogue = DialogueManager.Current.GetOrCreate<MessageDialogue>();
            dialogue.UsePresetForConfirmation( "قبول میکنی؟" );
        }

        [Button]
        async void MessageTestFullLoad() {
            var dialogue = DialogueManager.Current.GetOrCreate<MessageDialogue>();
            dialogue.SetLoadingLayoutActive( true );
            dialogue.AddButton( "بده", "ok" );
            dialogue.AddButton( "قبوله", "confirm" );
            dialogue.AddButton( "کنکله", "cancel" );
            dialogue.SetLoadingBarProgress( 0 );
            dialogue.SetBodyText( "الآن باید صفر باشه" );
            await Task.Delay( 2000 );
            dialogue.SetLoadingBarProgress( 0.7f );
            dialogue.SetBodyText( "زیاد شد. نه؟" );
            await Task.Delay( 2000 );
            dialogue.SetLoadingBarRotating();
            dialogue.SetBodyText( "حالا رفت رو حالت چرخشی!!! هورا داره کار میکنه (:" );
            await Task.Delay( 2000 );
            dialogue.SetLoadingBarProgress( 1 );
            dialogue.SetBodyText( "حالا فول شد" );
            await Task.Delay( 2000 );
            dialogue.SetBodyText( "اوکی ۳ ثانیه بعد میبندمش. ۳" );
            await Task.Delay( 1000 );
            dialogue.SetBodyText( "اوکی ۳ ثانیه بعد میبندمش. ۲" );
            await Task.Delay( 1000 );
            dialogue.SetBodyText( "اوکی ۳ ثانیه بعد میبندمش. ۱" );
            await Task.Delay( 1000 );
            await dialogue.Close();
        }
#endif
    }
}