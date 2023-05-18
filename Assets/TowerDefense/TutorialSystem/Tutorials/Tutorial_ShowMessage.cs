using System;
using System.Linq;
using DialogueSystem;
using RTLTMPro;
using TowerDefense.UI;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.TutorialSystem.Tutorials {
    public class Tutorial_ShowMessage : Tutorial {
        
        [Title("Show Message Params")]
        public RaycastBlock[] raycastBlocks;
        public Transform parentTransform;
        public MessageDialogueInfo[] messages;

        protected override async void ShowTutorial(Action onSuccess, Action onFail) {

            var prev = raycastBlocks.Select( rc => rc.raycaster.enabled ).ToList();
            foreach (var gr in raycastBlocks) gr.raycaster.enabled = false;

            try {
                foreach (var msg in messages) {
                    var d = DialogueManager.Current.GetOrCreate<MessageDialogue>( parentTransform );
                    d.SetTitleText( msg.title );
                    d.SetBodyText( msg.message );
                    d.AddButton( msg.confirmTxt, "ok" );
                    d.SetIcon( msg.iconType );
                    d.SetCloseButtonActive( !msg.shouldConfirm );
                    d.SetCanCloseByOutsideClick( !msg.shouldConfirm );
                    await d.AwaitClose();
                }

                onSuccess();
            }
            catch (Exception e) {
                Debug.LogException( e );
                onFail();
            }

            for (int i = 0; i < raycastBlocks.Length; i++)
                raycastBlocks[i].raycaster.enabled =
                    raycastBlocks[i].afterwards != RaycastBlock.AfterwardsFunction.BackToPreviousState || prev[i];
        }

        [Serializable]
        public class RaycastBlock {
            public GraphicRaycaster raycaster;
            public AfterwardsFunction afterwards;
            
            public enum AfterwardsFunction {
                BackToPreviousState, ForceEnable
            }
        }

        [Serializable]
        public class MessageDialogueInfo {
            [InfoBox( "$title_info" )] public string title;
            [TextArea( 3, 10 )]
            [InfoBox( "$message_info" )] public string message;
            [InfoBox( "$confirm_info" )] public string confirmTxt;
            public bool shouldConfirm;
            public MessageDialogue.IconType iconType;
            
#if UNITY_EDITOR
            [Button] void oneLinerMessage() => message = message.Replace( " \n", " " ).Replace( "\n", " " );
            string message_info => toEditorPer( message );
            string title_info => toEditorPer( title );
            string confirm_info => toEditorPer( confirmTxt );
            string toEditorPer(string txt) {
                var m = new FastStringBuilder( RTLSupport.DefaultBufferSize );
                RTLSupport.FixRTL( string.Join( "\n", txt.Split( '\n' ).Reverse()), m, true, false, false );
                return m.ToString();
            }
#endif
        }
    }
}