using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using TowerDefense.Data.Database;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;

namespace TowerDefense.TutorialSystem {
    public abstract class Tutorial : MonoBehaviour {
        
        [Tooltip("Context conditions will be checked less frequently, and if failed, detailed conditions will not be checked.")]
        [SerializeField] List<TutorialCondition> contextConditions;
        
        [Tooltip("If all context conditions are met, Detailed conditions will be checked every frame, and if met, " +
                 "the tutorial will be shown.")]
        [SerializeField] List<TutorialCondition> detailedConditions;
        
        [SerializeField] float contextCheckDelay = 0.5f;
        
        [SerializeField] string key;
        [SerializeField] UnityEvent executeIfShown;

        [Button]
        void moveDetailedConds2ContextCond() {
            contextConditions = new List<TutorialCondition>( detailedConditions );
            detailedConditions.Clear();
        }


        bool _showed = false;
        float _lastContextCheck = -1;
        bool _inContext = false;

        void OnEnable() => load();

        void save() => PreferencesDatabase.Current.Set( key, _showed ? 1 : 0 );
        void load() {
            if (PreferencesDatabase.Current.GetInt( key, out var val ))
                _showed = val == 1;
        }

        async void LateUpdate() {
            if (_showed) {
                executeIfShown?.Invoke();
                enabled = false; // just being sure
                return;
            }

            // update context every 0.5 seconds
            if (Time.unscaledTime - _lastContextCheck > contextCheckDelay) {
                _lastContextCheck = Time.unscaledTime;
                _inContext = isInContext();
            }

            // optimization: if not in context, don't check if it should be shown
            if (!_inContext) return;

            if (shouldShowTutorial()) {
                enabled = false; // don't re-check meanwhile
                // making sure scene is not unloading
                await Task.Delay( 200 );
                if ( !this || !shouldShowTutorial()) {
                    return;
                }
                var ts = Time.timeScale;
                Time.timeScale = 0;
                Debug.Log( $"showing tutorial: {key}" );
                ShowTutorial( () => {
                    Time.timeScale = ts;
                    _showed = true;
                    executeIfShown?.Invoke();
                    save();
                }, () => {
                    Time.timeScale = ts;
                    enabled = true;
                    _showed = false;
                    save();
                } );
            }
        }


        /// <summary>
        /// Whether or not the tutorial should be shown in the current context.
        /// </summary>
        bool isInContext() => contextConditions.All( c => c.IsMet() );

        /// <summary>
        /// Whether or not the tutorial should be shown, considering it's in the right context
        /// </summary>
        bool shouldShowTutorial() => detailedConditions.All( c => c.IsMet() );

        protected abstract void ShowTutorial([NotNull] Action onSuccess, [NotNull] Action onFail);
    }
}