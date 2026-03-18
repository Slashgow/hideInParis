using System.Collections.Generic;
using System.Linq;
using inkolorgames;
using inkolorgames.effects;
using UnityEngine;

public class SFXManager : BaseSFXManager
{
    [SerializeField] private AudioClip hoverAudioClip, pressedAudioClip;

    [Header("Surfaces")]
    [SerializeField] private List<AudioClipSurface> audioClipsSurface;

    private void OnEnable()
    {
        SelectableInputRegister.OnAnySelectableHover += SelectableInputRegister_OnAnySelectableHover;
        SelectableInputRegister.OnAnySelectablePressed += SelectableInputRegister_OnAnySelectablePressed;
        ClickManager.OnClickOnSurface += ClickManager_OnClickOnSurface;
    }

    private void OnDisable()
    {
        SelectableInputRegister.OnAnySelectableHover -= SelectableInputRegister_OnAnySelectableHover;
        SelectableInputRegister.OnAnySelectablePressed -= SelectableInputRegister_OnAnySelectablePressed;
        ClickManager.OnClickOnSurface -= ClickManager_OnClickOnSurface;
    }

    private void SelectableInputRegister_OnAnySelectablePressed() => PlayAudioClip(pressedAudioClip);
    private void SelectableInputRegister_OnAnySelectableHover() => PlayAudioClip(hoverAudioClip);
    private void ClickManager_OnClickOnSurface(SurfaceType surfaceType) => PlayAudioClip(GetAudioClipBySurface(surfaceType));

    private AudioClip GetAudioClipBySurface(SurfaceType surfaceType)
    {
        return audioClipsSurface.FirstOrDefault(audioClipSurface => audioClipSurface.SurfaceType == surfaceType).GetRandomAudioClip();
    }
}

