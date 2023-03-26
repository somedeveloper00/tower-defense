using TowerDefense.Core.Starter;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Main {
    public class MainMenu : MonoBehaviour {

        public bool useJson = true;

        [ShowIf( nameof(useJson), false )] public CoreStarter coreStarter;
        [ShowIf( nameof(useJson), true ), TextArea] public string json;
        
        [Button(ButtonSizes.Large)]
        public void StartGame() {
            if (useJson) {
                coreStarter = ScriptableObject.CreateInstance<CoreStarter>();
                coreStarter.FromJson( json );
            }
            StartCoroutine( coreStarter.StartGame() );
        }
    }
}