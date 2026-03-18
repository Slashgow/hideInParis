using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityTimer;


namespace inkolorgames
{
    public abstract class BaseMusicManager : MonoSingleton<BaseMusicManager>
    {
        [SerializeField] private Logger logger;

        [Header("Audio Sources")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioSource secondaryAudioSource;

        [Header("Audio Clips")]
        [SerializeField] private List<AudioClip> musics;

        [Header("Crossfade Settings")]
        [SerializeField, Range(0f, 10f)] private float crossfadeDuration = 2f;

        [Header("Pause Settings")]
        [SerializeField, Range(0f, 5f)] private float fadeDuration = 1f;
        [SerializeField] private Ease fadeEase = Ease.InOutQuad;
        [SerializeField, Range(-3f, 3f)] private float pausePitch = 0.3f;
        [SerializeField, Range(-3f, 3f)] private float resumePitch = 1f;

        private Tween fadeTween;
        private Tween volumeFadeOutTween;
        private Tween volumeFadeInTween;
        private Timer musicEndTimer;
        private int currentMusicIndex = 0;
        public AudioSource CurrentAudioSource { get; private set; }

        protected virtual void PlayMusicAtIndex(int index)
        {
            if (musics.Count == 0)
                return;

            currentMusicIndex = index % musics.Count;
            AudioClip clipToPlay = musics[currentMusicIndex];

            if (clipToPlay == null)
            {
                logger.Log($"Music at index {currentMusicIndex} is null, skipping to next", this);
                PlayNextMusic();
                return;
            }

            if (CurrentAudioSource == null)
            {
                // First time playing - no crossfade needed
                CurrentAudioSource = audioSource;
                CurrentAudioSource.clip = clipToPlay;
                CurrentAudioSource.volume = 1f;
                CurrentAudioSource.pitch = resumePitch;
                CurrentAudioSource.Play();
                logger.Log($"Started playing music: {clipToPlay.name}", this);
            }
            else
            {
                // Crossfade to next music
                CrossfadeToClip(clipToPlay, crossfadeDuration, false);
            }

            // Schedule next music to play before this one ends (accounting for crossfade)
            float timeUntilCrossfade = clipToPlay.length - crossfadeDuration;
            timeUntilCrossfade = Mathf.Max(0f, timeUntilCrossfade); // Ensure it's not negative

            musicEndTimer?.Cancel();
            musicEndTimer = Timer.Register(timeUntilCrossfade, PlayNextMusic);

            logger.Log($"Next music will start in {timeUntilCrossfade}s", this);
        }
        private void PlayNextMusic()
        {
            int nextIndex = (currentMusicIndex + 1) % musics.Count;
            PlayMusicAtIndex(nextIndex);
        }

        public void PitchDownMusic()
        {
            fadeTween?.Kill();
            fadeTween = CurrentAudioSource.DOPitch(pausePitch, fadeDuration).SetEase(fadeEase).SetUpdate(true);
            logger.Log("Music pitch down", this);
        }

        public void ResumePitchMusic()
        {
            fadeTween?.Kill();
            fadeTween = CurrentAudioSource.DOPitch(resumePitch, fadeDuration).SetEase(fadeEase).SetUpdate(true);
            logger.Log("Music pitch resume", this);
        }

        public void CrossfadeToClip(AudioClip newClip, float duration, bool loopNext)
        {
            if (newClip == null)
            {
                logger.Log("Cannot crossfade to null clip", this);
                return;
            }

            volumeFadeOutTween?.Kill();
            volumeFadeInTween?.Kill();

            AudioSource fadeOutSource = CurrentAudioSource;
            AudioSource fadeInSource = CurrentAudioSource == audioSource ? secondaryAudioSource : audioSource;

            fadeInSource.clip = newClip;
            fadeInSource.volume = 0f;
            fadeInSource.pitch = CurrentAudioSource.pitch;

            if (loopNext)
                fadeInSource.loop = true;

            fadeInSource.Play();

            volumeFadeOutTween = fadeOutSource.DOFade(0f, duration)
                .SetEase(fadeEase)
                .SetUpdate(true)
                .OnComplete(() => fadeOutSource.Stop());

            volumeFadeInTween = fadeInSource.DOFade(1f, duration)
                .SetEase(fadeEase)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    CurrentAudioSource = fadeInSource;
                    logger.Log($"Crossfaded to clip: {newClip.name} over {duration}s", this);
                });

            logger.Log($"Starting crossfade to clip: {newClip.name} over {duration}s", this);
        }
        protected virtual void OnDestroy()
        {
            fadeTween?.Kill();
            volumeFadeOutTween?.Kill();
            volumeFadeInTween?.Kill();
            musicEndTimer?.Cancel();
        }

    }
}