using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TowerDefense.Data.Database;
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
        

        bool _showed = false;
        float _lastContextCheck = -1;
        bool _inContext = false;

        void OnEnable() => load();

        void save() => PreferencesDatabase.Current.Set( key, _showed ? 1 : 0 );
        void load() {
            if (PreferencesDatabase.Current.GetInt( key, out var val ))
                _showed = val == 1;
        }

        void LateUpdate() {
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
                Time.timeScale = 0;
                ShowTutorial( () => {
                    Time.timeScale = 1;
                    _showed = true;
                    executeIfShown?.Invoke();
                    save();
                }, () => {
                    Time.timeScale = 1;
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