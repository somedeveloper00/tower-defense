using System;
using AnimFlex.Sequencer.UserEnd;
using AnimFlex.Tweening;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Common {
    public class LoadingBar : MonoBehaviour {
        [SerializeField] RectTransform bar;
        [SerializeField] Rect zeroRect, fullRect;
        [SerializeField] float duration = 0.2f;
        [SerializeField] SequenceAnim loadingSeq;
        [SerializeField] GameObject rotatingLayout;
        [SerializeField] GameObject progressiveLayout;

        Tweener _tweener;

        /// <summary>
        /// between 0 and 1
        /// </summary>
        /// <param name="t"></param>
        public void SetProgress(float t) {
            StopRotatingAnim();
            float val = 0;
            var posFrom = bar.anchoredPosition;
            var posTo = Vector2.Lerp( zeroRect.position, fullRect.position, t );
            var sizeFrom = bar.sizeDelta;
            var sizeTo = Vector2.Lerp( zeroRect.size, fullRect.size, t );
            // animate
            if (_tweener?.IsActive() ?? false) _tweener.Kill( false, false );
            _tweener = Tweener.Generate(
                () => val,
                (value) => {
                    val = value;
                    bar.anchoredPosition = Vector2.Lerp( posFrom, posTo, val );
                    bar.sizeDelta = Vector2.Lerp( sizeFrom, sizeTo, val );
                },
                1, duration: duration, isValid: () => this && bar );
        }

        public void PlayRotatingAnim() {
            if (loadingSeq.sequence is null || !loadingSeq.sequence.IsPlaying()) {
                rotatingLayout.SetActive( true );
                progressiveLayout.SetActive( false );
                loadingSeq.PlaySequence();
            }
        }
        
        public void StopRotatingAnim() {
            if (loadingSeq.sequence.IsPlaying()) {
                loadingSeq.sequence.Stop();
                loadingSeq.sequence = null;
            }
            rotatingLayout.SetActive( false );
            progressiveLayout.SetActive( true );
        }

        [Button] void captureZeroState() => zeroRect = new Rect( bar.anchoredPosition, bar.sizeDelta );
        [Button] void captureFullState() => fullRect = new Rect( bar.anchoredPosition, bar.sizeDelta );
    }
}