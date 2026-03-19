using System.Collections.Generic;
using System.Linq;
using inkolorgames;
using inkolorgames.effects;
using UnityEngine;

public class SFXManager : BaseSFXManager
{
    [SerializeField] private AudioClip hoverAudioClip, pressedAudioClip;
    [SerializeField] private AudioClip foundOrPlaceItemAudioClip;
    [Header("Surfaces")]
    [SerializeField] private List<AudioClipSurface> audioClipsSurface;

    private void OnEnable()
    {
        SelectableInputRegister.OnAnySelectableHover += SelectableInputRegister_OnAnySelectableHover;
        SelectableInputRegister.OnAnySelectablePressed += SelectableInputRegister_OnAnySelectablePressed;
        ClickManager.OnClickOnSurface += ClickManager_OnClickOnSurface;
        HiddenObjectManager.OnAnyItemFound += HiddenObjectManager_OnAnyItemFound;
        HiddenObjectManager.OnAnyItemPlaced += HiddenObjectManager_OnAnyItemPlaced;
    }

    private void OnDisable()
    {
        SelectableInputRegister.OnAnySelectableHover -= SelectableInputRegister_OnAnySelectableHover;
        SelectableInputRegister.OnAnySelectablePressed -= SelectableInputRegister_OnAnySelectablePressed;
        ClickManager.OnClickOnSurface -= ClickManager_OnClickOnSurface;
        HiddenObjectManager.OnAnyItemFound -= HiddenObjectManager_OnAnyItemFound;
        HiddenObjectManager.OnAnyItemPlaced -= HiddenObjectManager_OnAnyItemPlaced;
    }

    private void SelectableInputRegister_OnAnySelectablePressed() => PlayAudioClip(pressedAudioClip);
    private void SelectableInputRegister_OnAnySelectableHover() => PlayAudioClip(hoverAudioClip);
    private void ClickManager_OnClickOnSurface(SurfaceType surfaceType) => PlayAudioClip(GetAudioClipBySurface(surfaceType));
    private void HiddenObjectManager_OnAnyItemPlaced(string obj) => PlayAudioClip(foundOrPlaceItemAudioClip);
    private void HiddenObjectManager_OnAnyItemFound(string arg1, int arg2) => PlayAudioClip(foundOrPlaceItemAudioClip);
    private AudioClip GetAudioClipBySurface(SurfaceType surfaceType)
    {
        return audioClipsSurface.FirstOrDefault(audioClipSurface => audioClipSurface.SurfaceType == surfaceType).GetRandomAudioClip();
    }
}

