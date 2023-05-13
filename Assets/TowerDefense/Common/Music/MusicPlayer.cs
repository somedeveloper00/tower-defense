using System;
using System.Threading.Tasks;
using AnimFlex;
using AnimFlex.Core.Proxy;
using AnimFlex.Tweening;
using TowerDefense.Data.Database;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Music {
    [RequireComponent(typeof(AudioSource))]
    public class MusicPlayer : MonoBehaviour {
        [SerializeField, Range(0, 1)] float volume = 1;
        [SerializeField] float fadeDuration = 1;
        [SerializeField] bool SaveMuteState = true;

        [ShowIf( nameof(SaveMuteState), true )]
        [SerializeField] string prefMuteKey = "mute";

        AudioSource source;
        Tweener _anim;
        bool _muted = false;

        public event Action onLoaded;

        void Start() {
            source = GetComponent<AudioSource>();
            loadState();
        }

        public bool IsMuted() => _muted;
        public void Mute(bool save = false) => setMuteState( true, true, save );
        public void Unmute(bool save = false) => setMuteState( false, true, save );

        public async Task AwaitFade() {
            if (_anim is null || !_anim.IsActive()) return;
            await _anim.AwaitComplete();
        }

        void loadState() {
            if (PreferencesDatabase.Current.TryGetInt( prefMuteKey, out var result )) {
                setMuteState( result == 1, false, false );
            }
            else {
                setMuteState( false, true, true );
            }
            onLoaded?.Invoke();
        }

        void saveState() {
            if (PreferencesDatabase.Current is not null) {
                PreferencesDatabase.Current.Set( prefMuteKey, _muted ? 1 : 0 );
            }
            else {
                Debug.LogError( $"Prefs database not found!" );
            }
        }


        void setMuteState(bool muted, bool fade, bool save) {
            _muted = muted;
            if (fade) {
                if (_anim is not null && _anim.IsActive()) _anim.Kill( false, false );
                _anim = source.AnimAudioVolumeTo( muted ? 0 : volume, duration: fadeDuration, proxy: AnimFlexCoreProxyUnscaled.Default );
            }
            else {
                source.volume = muted ? 0 : volume;
            }
            if (save) {
                saveState();
            }
        }
    }
}