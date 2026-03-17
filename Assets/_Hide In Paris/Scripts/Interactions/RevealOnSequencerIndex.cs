using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Drop this on any GameObject to hide/show it based on a SpriteSequencer's current index.
/// Disables the SpriteRenderer and all Collider2Ds when hidden.
/// </summary>
public class RevealOnSequencerIndex : MonoBehaviour
{
    [Tooltip("The SpriteSequencer to watch.")]
    public SpriteSequencer sequencer;

    [Tooltip("This object becomes visible/interactable when the sequencer reaches this index.")]
    public int revealAtIndex = 1;

    [Tooltip("If true, hides again when the sequencer goes back below the reveal index.")]
    public bool hideOnRevert = true;

    private SpriteRenderer[] _spriteRenderers;
    private Collider2D[] _colliders;
    private HiddenObjectItem _hiddenObject;

    private void Awake()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        _colliders = GetComponents<Collider2D>();
        _hiddenObject = GetComponent<HiddenObjectItem>();

        if (sequencer == null)
        {
            Debug.LogWarning($"[RevealOnSequencerIndex] No sequencer assigned on {gameObject.name}.", this);
            return;
        }

        sequencer.onIndexChanged.AddListener(OnIndexChanged);

        // Apply immediately based on current sequencer state
        Apply(sequencer.CurrentIndex >= revealAtIndex);
    }

    private void OnDestroy()
    {
        if (sequencer != null)
            sequencer.onIndexChanged.RemoveListener(OnIndexChanged);
    }

    private void OnIndexChanged(int newIndex)
    {
        bool shouldReveal = newIndex >= revealAtIndex;

        if (!shouldReveal && !hideOnRevert) return;

        Apply(shouldReveal);
    }

    private void Apply(bool revealed)
    {
        if (_spriteRenderers != null)
        {
            foreach (var spriteRenderer in _spriteRenderers)
            {
                spriteRenderer.enabled = revealed;
            }
        }

        if(_hiddenObject != null)
            _hiddenObject.enabled = revealed;

        foreach (var col in _colliders)
            col.enabled = revealed;
    }
}