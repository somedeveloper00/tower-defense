using System;
using System.Threading.Tasks;
using TowerDefense.Ad;
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

        public async void StartLoadingScreen() {
            if (t >= 0 || _currentUi != null) return;
            Debug.Log( $"started loading" );
            _currentUi = Instantiate( prefab, transform );
            _currentUi.canvas.gameObject.SetActive( true );
            _currentUi.inSequence.PlaySequence();
            t = 0;

            // setup banner ad
            if (AdManager.Current is not null && await AdManager.Current.IsInitialized())
                await AdManager.Current.ShowSidedBannerAd( "642fee1e2eeae447e5ae5bc9" );
        }

        public bool IsON() => t >= 0 || _currentUi != null; 

        public async void EndLoadingScreen() {
            if (t < minScreenDuration) {
                await Task.Delay( (int)(1000 * (minScreenDuration - t)) );
            }

            t = -1;
            Debug.Log( $"ended loading" );
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
                _currentUi.DestroyGameObject();
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
            
            // remove banner 
            if (AdManager.Current is not null && await AdManager.Current.IsInitialized())
                await AdManager.Current.RemoveSidedBannerAd( "642fee1e2eeae447e5ae5bc9" );
        }
        
        
        public enum State {
            Undefined, StartingGame, GoingToCore, BackFromCore, RestartingCore
        }
    }
}