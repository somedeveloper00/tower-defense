using System;
using System.Threading.Tasks;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Background.Loading {
    [DeclareFoldoutGroup("ref", Title = "References", Expanded = true)]
    public class LoadingScreenManager : MonoBehaviour {
        
        public static LoadingScreenManager Current;

        public State state;
        public Action onEndAnimComplete, onEndAnimCompleteOnce;
        public Action onEndAnimStart, onEndAnimStartOnce;

        [SerializeField] float minScreenDuration = 2;

        [GroupNext( "ref" )] 
        [SerializeField] Transform parentCanavs;
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

        public RectTransform GetTransform() => (RectTransform)_currentUi.transform;
        public Canvas GetCanvas() => _currentUi.canvas;

        public void StartLoadingScreen() {
            if (t >= 0 || _currentUi != null) return;
            _currentUi = Instantiate( prefab, parentCanavs );
            _currentUi.canvas.gameObject.SetActive( true );
            _currentUi.inSequence.PlaySequence();
            t = 0;
        }

        public bool IsON() => t >= 0 || _currentUi != null; 

        public async void EndLoadingScreen() {
            // wait if it was too quick
            if (t < minScreenDuration) {
                await Task.Delay( (int)( 1000 * ( minScreenDuration - t ) ) );
            }

            t = -1;
            Debug.Log( $"loading ending" );
            if (_currentUi != null && _currentUi.inSequence.sequence is not null &&
                _currentUi.inSequence.sequence.IsPlaying())
                _currentUi.inSequence.sequence.Stop();

            try {
                onEndAnimStart?.Invoke();
                if (onEndAnimStartOnce is not null) {
                    onEndAnimStartOnce?.Invoke();
                    onEndAnimStartOnce = null;
                }
            }
            catch (Exception e) {
                Debug.LogException( e );
            }
            
            _currentUi.outSequence.PlaySequence();
            _currentUi.outSequence.sequence.onComplete += () => {
                _currentUi.Close();
                try {
                    onEndAnimComplete?.Invoke();
                    if (onEndAnimCompleteOnce is not null) {
                        onEndAnimCompleteOnce?.Invoke();
                        onEndAnimCompleteOnce = null;
                    }
                }
                catch (Exception e) {
                    Debug.LogException( e );
                }
            };
            Debug.Log( $"loading finished ending" );
        }
        
        
        public enum State {
            Undefined, StartingGame, GoingToCore, BackFromCore, RestartingCore
        }
    }
}