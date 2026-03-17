using UnityEngine;
using UnityEngine.Events;

public class SpriteSequencer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Sprites")]
    [Tooltip("Sprites cycled through on each click. Index 0 is the default/idle sprite.")]
    public Sprite[] sprites;

    [SerializeField] private bool showFirstFrameOnAwake;

    public enum EndBehaviour
    {
        StopOnLast,  
        Loop,        
    }

    [Tooltip("What happens when the last sprite in the sequence is reached.")]
    public EndBehaviour endBehaviour = EndBehaviour.StopOnLast;

    [Header("Events")]
    [Tooltip("Fired on every click (before the sprite changes). Passes the new index.")]
    public UnityEvent<int> onIndexChanged;

    [Tooltip("Fired once when the last sprite is reached (both StopOnLast and the first time Loop wraps).")]
    public UnityEvent onSequenceCompleted;


    private int _currentIndex = -1;
    private bool _locked = false;
    private bool _completedFired = false;

    public int CurrentIndex => _currentIndex;
    public bool IsLocked => _locked;


    private void Awake()
    {
        if (!showFirstFrameOnAwake)
        {
            spriteRenderer.enabled = false;
            return;
        }
           
        ApplySprite(0);
    }

    private void OnMouseDown()
    {
        if (_locked) return;
        if (sprites == null || sprites.Length == 0) return;

        Advance();
    }


    private void Advance()
    {
        if (!spriteRenderer.enabled)
            spriteRenderer.enabled = true;

        int next = _currentIndex + 1;
        bool reachedEnd = next >= sprites.Length;

        bool isEnd = next >= sprites.Length -1;
        if (isEnd)
            FireCompleted();

        if (reachedEnd)
        {
            switch (endBehaviour)
            {
                case EndBehaviour.StopOnLast:
                    next = sprites.Length - 1;
                    ApplySprite(next);
                    Lock();
                    //FireCompleted();
                    break;

                case EndBehaviour.Loop:
                    next = 0;
                    ApplySprite(next);
                    //FireCompleted();  
                    break;
            }
        }
        else
        {
            ApplySprite(next);
        }
    }

    private void ApplySprite(int index)
    {
        _currentIndex = index;
        if (index < sprites.Length)
            spriteRenderer.sprite = sprites[index];

        onIndexChanged?.Invoke(_currentIndex);
    }

    private void FireCompleted()
    {
        if (_completedFired) 
            return;

        _completedFired = true;
        onSequenceCompleted?.Invoke();
    }


    public void Lock() => _locked = true;
    public void Unlock() => _locked = false;
    public void SetIndex(int index)
    {
        index = Mathf.Clamp(index, 0, sprites.Length - 1);
        ApplySprite(index);
    }
    public void ResetSequencer()
    {
        _locked = false;
        _completedFired = false;
        ApplySprite(0);
    }
}