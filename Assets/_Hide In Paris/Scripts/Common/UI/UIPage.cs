using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent (typeof(CanvasGroup))]
public class UIPage : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public event Action OnShow;
    public UnityEvent OnShowUnityEvent;
    public UnityEvent OnHideUnityEvent;
    protected virtual void Awake() => canvasGroup = GetComponent<CanvasGroup>();
    public virtual void Show() 
    { 
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
        OnShow?.Invoke();
        OnShowUnityEvent?.Invoke();
    }
    public virtual void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        OnHideUnityEvent?.Invoke();
    }
}
