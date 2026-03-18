using System.Collections.Generic;
using UnityEngine;


namespace inkolorgames
{
    public abstract class BaseSFXManager : MonoSingleton<BaseSFXManager>
    {
        [Header("References")]
        [SerializeField] protected List<AudioSource> audioSources = new List<AudioSource>();

        protected int currentAudioSourceIndex;

        public virtual void PlayAudioClip(AudioClip audioClip) => PlayAudioClipWithPitch(audioClip, 1f);
        public virtual void PlayRandomAudioClip(List<AudioClip> audioClips)
        {
            int randomIndex = Random.Range(0, audioClips.Count);
            AudioClip randomClip = audioClips[randomIndex];
            PlayAudioClip(randomClip);
        }

        public virtual void PlayAudioClipWithPitch(AudioClip audioClip, float pitch)
        {
            AudioSource source = audioSources[currentAudioSourceIndex];
            source.clip = audioClip;
            source.pitch = pitch;
            source.Play();
            currentAudioSourceIndex = (currentAudioSourceIndex + 1) % audioSources.Count;
        }
    }
}

