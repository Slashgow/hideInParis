using System;
using DG.Tweening;
using inkolorgames;
using UnityEngine;

public class HintManager : MonoSingleton<HintManager>
{
    [SerializeField, Range(0,5)] private int maxHintCount;

    [Header("Camera")]
    [SerializeField] private CameraMover cameraMover;
    [SerializeField, Range(0f,15f)] private float cameraZoomHint = 5f;
    [SerializeField, Range(0f,2f)] private float cameraMovementDuration = 1f;

    [Header("Highlight")]
    [SerializeField] private Color highlightColor;
    [SerializeField, Range(0f, 3f)] private float hihglightDuration;
    [SerializeField, Range(0, 10)] private int overshoot = 10;
    
    private int remainingHintCount;
    public int RemainingHintCount => remainingHintCount;

    public static event Action<int> OnUseHint;

    protected override void Awake()
    {
        base.Awake();

        remainingHintCount = maxHintCount;
    }

    public void ShowHint()
    {
        if (remainingHintCount <= 0)
            return;

        remainingHintCount--;
        OnUseHint?.Invoke(remainingHintCount);

        HiddenObjectItem hiddenObjectItem = HiddenObjectManager.Instance.GetNextUnfoundHiddenObject();

        if(hiddenObjectItem != null)
        {
            Vector3 targetPosition = new Vector3(hiddenObjectItem.transform.position.x, hiddenObjectItem.transform.position.y, cameraMover.transform.position.z);
            cameraMover.MoveAndZoomToSameTime(cameraMovementDuration, cameraMovementDuration, cameraZoomHint, targetPosition);
            hiddenObjectItem.SpriteRenderer.DOColor(Color.black, hihglightDuration).SetEase(Ease.Flash, overshoot, 0);
            return;
        }

        HiddenObjectDropZone hiddenObjectDropZone = HiddenObjectManager.Instance.GetNextDropZoneItemNotPlaced();

        if (hiddenObjectDropZone != null)
        {
            Vector3 targetPosition = new Vector3(hiddenObjectDropZone.transform.position.x, hiddenObjectDropZone.transform.position.y, cameraMover.transform.position.z);
            cameraMover.MoveAndZoomToSameTime(cameraMovementDuration, cameraMovementDuration, cameraZoomHint, targetPosition);
        }
    }
}
