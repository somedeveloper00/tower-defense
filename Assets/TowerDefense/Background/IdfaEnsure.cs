using System.Threading.Tasks;
using DialogueSystem;
using TowerDefense.Background.Loading;
using TowerDefense.Common;
using TowerDefense.Data.Database;
using UnityEngine;

namespace TowerDefense.Background {
    public class IdfaEnsure : MonoBehaviour {
        [SerializeField] string messageTitleTxt;
        [SerializeField, TextArea] string messageBodyTxt;
        [SerializeField] string okTxt;

        void Start() {
            GameInitializer.Current.onInitTasks.Add( new GameInitializer.OnInitTask( 10, EnsureIDFA()) );
        }

        public async Task EnsureIDFA() {
            if (!PreferencesDatabase.Current.KeyExists( "idfa" )) {
                var dialogue =
                    DialogueManager.Current.GetOrCreate<MessageDialogue>( LoadingScreenManager.Current.GetTransform().parent );
                dialogue.SetTitleText( messageTitleTxt );
                dialogue.SetBodyText( messageBodyTxt );
                dialogue.AddButton( okTxt, "ok" );
                dialogue.SetCanCloseByOutsideClick( false );
                dialogue.SetCloseButtonActive( false );
                await dialogue.AwaitClose();
                PreferencesDatabase.Current.Set( "idfa", 1 );
            }

            Debug.Log( $"IDFA check done" );
        }
    }
}