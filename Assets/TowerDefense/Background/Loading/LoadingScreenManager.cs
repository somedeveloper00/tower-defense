using System;
using System.Threading.Tasks;
using AnimFlex.Sequencer.UserEnd;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Background.Loading {
    [DeclareFoldoutGroup("ref", Title = "References", Expanded = true)]
    public class LoadingScreenManager : MonoBehaviour {
        public static LoadingScreenManager Current;

        public Action onEndAnimComplete;
        public Action onEndAnimStart;

        [SerializeField] float minScreenDuration = 2;

        [GroupNext( "ref" )] 
        [SerializeField] LoadingSceneUI prefab;
        
        [ShowInInspector, ReadOnly]
        float t = -1;

        LoadingSceneUI _currentUi;
        
        void OnEnable() {
            Current = this;
        }

        void Update() {
            if (t >= 0) t += Time.deltaTime;
        }

        public void StartLoadingScreen() {
            if (t >= 0 || _currentUi != null) return;
            Debug.Log( $"started loading" );
            _currentUi = Instantiate( prefab, transform );
            _currentUi.canvas.gameObject.SetActive( true );
            _currentUi.inSequence.PlaySequence();
            t = 0;
        }

        public bool IsON() => t >= 0 || _currentUi != null; 

        public async void EndLoadingScreen() {
            if (t < minScreenDuration) {
                Debug.Log( $"ending loading in {minScreenDuration - t} s" );
                await Task.Delay( (int)(1000 * (minScreenDuration - t)) );
            }

            t = -1;
            Debug.Log( $"ended loading" );
            _currentUi.inSequence.sequence.Stop();

            try {
                onEndAnimStart?.Invoke();
            }
            catch (Exception e) {
                Debug.LogException( e );
            }
            
            _currentUi.outSequence.PlaySequence();
            _currentUi.outSequence.sequence.onComplete += () => {
                _currentUi.DestroyGameObject();
                try {
                    onEndAnimComplete?.Invoke();
                }
                catch (Exception e) {
                    Debug.LogException( e );
                }
            };
        }
    }
}