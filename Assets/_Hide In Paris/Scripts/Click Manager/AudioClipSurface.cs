using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AudioClipSurface
{
    [SerializeField] private List<AudioClip> audioClips = new List<AudioClip>();
    [SerializeField] private SurfaceType surfaceType;
    public SurfaceType SurfaceType => surfaceType;
    public AudioClip GetRandomAudioClip() => audioClips[UnityEngine.Random.Range(0, audioClips.Count-1)];
}
