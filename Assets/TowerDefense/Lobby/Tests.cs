// ReSharper disable Unity.NoNullPropagation
using System.Collections;
using System.Threading.Tasks;
using DialogueSystem;
using TowerDefense.Background;
using TowerDefense.Common;
using TowerDefense.Data.Database;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Lobby {
    public class Tests : MonoBehaviour {
        public Button fullTest, deleteAll;

        void Start() {
            fullTest?.onClick.AddListener( () => StartCoroutine( MessageTestFullLoad() ) );
            deleteAll?.onClick.AddListener( DeleteSaveData );
        }

        public void deleteData() {
            SecureDatabase.Current.DeleteAll();
            PreferencesDatabase.Current.DeleteAll();
            SecureDatabase.Current.Load();
            PreferencesDatabase.Current.Load();
        }

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

        void DeleteSaveData() {
            SecureDatabase.Current.DeleteAll();
        }

        [Button]
        IEnumerator MessageTestFullLoad() {
            var dialogue = DialogueManager.Current.GetOrCreate<MessageDialogue>();
            dialogue.SetLoadingLayoutActive( true );
            dialogue.AddButton( "بده", "ok" );
            dialogue.AddButton( "قبوله", "confirm" );
            dialogue.AddButton( "کنکله", "cancel" );
            dialogue.SetLoadingBarProgress( 0 );
            dialogue.SetBodyText( "الآن باید صفر باشه" );
            yield return new WaitForSeconds( 2 );
            dialogue.SetLoadingBarProgress( 0.7f );
            dialogue.SetBodyText( "زیاد شد. نه؟" );
            yield return new WaitForSeconds( 2 );
            dialogue.SetLoadingBarRotating();
            dialogue.SetBodyText( "حالا رفت رو حالت چرخشی!!! هورا داره کار میکنه (:" );
            yield return new WaitForSeconds( 2 );
            dialogue.SetLoadingBarProgress( 1 );
            dialogue.SetBodyText( "حالا فول شد" );
            yield return new WaitForSeconds( 2 );
            dialogue.SetBodyText( "اوکی ۳ ثانیه بعد میبندمش. ۳" );
            yield return new WaitForSeconds( 1 );
            dialogue.SetBodyText( "اوکی ۳ ثانیه بعد میبندمش. ۲" );
            yield return new WaitForSeconds( 1 );
            dialogue.SetBodyText( "اوکی ۳ ثانیه بعد میبندمش. ۱" );
            yield return new WaitForSeconds( 1 );
            yield return new WaitForTask( dialogue.Close() );
        }
    }
}