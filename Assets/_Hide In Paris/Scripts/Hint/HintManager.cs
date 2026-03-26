using System;
using DG.Tweening;
using inkolorgames;
using UnityEngine;
using UnityTimer;

public class HintManager : MonoSingleton<HintManager>
{
    public enum HintCategory
    {
        LARGE_ZONE,
        SMALL_ZONE,
        EXACT_ZONE
    }

    [SerializeField, Range(0f, 60f)] private float reloadTimeHint = 30f;
    [SerializeField, Range(0,5)] private int startHintCount;

    [Header("Cost")]
    [SerializeField, Range(0, 5)] private int largeZoneCost = 1;
    [SerializeField, Range(0, 5)] private int smallZoneCost = 2;
    [SerializeField, Range(0, 5)] private int exactZoneCost = 3;


    [Header("Camera")]
    [SerializeField] private CameraMover cameraMover;
    [SerializeField, Range(0f,15f)] private float cameraExactZoneZoomHint = 5f;
    [SerializeField, Range(0f, 15f)] private float cameraSmallZoneZoomHint = 8f;
    [SerializeField, Range(0f, 15f)] private float cameraLargeZoneZoomHint = 12f;
    [SerializeField, Range(0f,2f)] private float cameraMovementDuration = 1f;

    [Header("Highlight")]
    [SerializeField] private Color highlightColor;
    [SerializeField, Range(0f, 3f)] private float hihglightDuration;
    [SerializeField, Range(0, 10)] private int overshoot = 10;

    [Header("Zone Circle Hints")]
    [SerializeField] private HintCircleRenderer hintCircleRenderer;
    [SerializeField, Range(0.5f, 20f)] private float largeZoneRadius = 8f;
    [SerializeField, Range(0.5f, 10f)] private float smallZoneRadius = 4f;
    [SerializeField, Range(0.5f, 10f)] private float exactZoneRadius = 2f;
    [SerializeField, Range(0f, 5f)] private float largeZoneMaxOffset = 3f;
    [SerializeField, Range(0f, 3f)] private float smallZoneMaxOffset = 1.5f;


    private int remainingHintCount;

    public int LargeZoneCost => largeZoneCost;
    public int SmallZoneCost => smallZoneCost;
    public int ExactZoneCost => exactZoneCost;
    public int RemainingHintCount => remainingHintCount;

    public static event Action<int> OnHintCountUpdated;
    public static event Action<float> OnReloadTimerTick;
    private Timer reloadHintTimer;

    private int lastWholeSecondRemaining = -1;

    protected override void Awake()
    {
        base.Awake();

        remainingHintCount = startHintCount;
        reloadHintTimer = Timer.Register(reloadTimeHint, onComplete: OnReloadTimerEnd, onUpdate: TickReload, isLooped: true);
    }

    private void TickReload(float timeElapsed)
    {
        float timeRemaining = reloadTimeHint - timeElapsed;
        int wholeSecondsRemaining = Mathf.CeilToInt(timeRemaining);

        if (wholeSecondsRemaining == lastWholeSecondRemaining)
            return;

        lastWholeSecondRemaining = wholeSecondsRemaining;
        OnReloadTimerTick?.Invoke(timeElapsed / reloadTimeHint);
    }

    private void OnDestroy() => reloadHintTimer?.Cancel();
    private void OnReloadTimerEnd()
    {
        remainingHintCount++;
        OnHintCountUpdated?.Invoke(remainingHintCount);
    }

    public void ShowHint(string groupID, HintCategory hintCategory)
    {
        Vector3 targetWorldPos;
        bool foundTarget = TryGetHintTarget(groupID, out targetWorldPos);

        if (!foundTarget)
            return;

        if (!TryDeductCost(hintCategory))
            return;

        switch (hintCategory)
        {
            case HintCategory.LARGE_ZONE:
                ShowZoneCircle(targetWorldPos, largeZoneRadius, largeZoneMaxOffset, cameraLargeZoneZoomHint);
                break;

            case HintCategory.SMALL_ZONE:
                ShowZoneCircle(targetWorldPos, smallZoneRadius, smallZoneMaxOffset, cameraSmallZoneZoomHint);
                break;

            case HintCategory.EXACT_ZONE:
                ShowZoneCircle(targetWorldPos, exactZoneRadius, 0f, cameraExactZoneZoomHint);
                break;
        }
    }

    public void ShowNextRandomHint()
    {
        if (remainingHintCount <= 0)
            return;

        remainingHintCount--;
        OnHintCountUpdated?.Invoke(remainingHintCount);

        HiddenObjectItem hiddenObjectItem = HiddenObjectManager.Instance.GetNextUnfoundHiddenObject();

        if (hiddenObjectItem != null)
        {
            ShowExactZoom(hiddenObjectItem.transform.position);
            hiddenObjectItem.SpriteRenderer.DOColor(Color.black, hihglightDuration).SetEase(Ease.Flash, overshoot, 0);
            return;
        }

        HiddenObjectDropZone hiddenObjectDropZone = HiddenObjectManager.Instance.GetNextDropZoneItemNotPlaced();

        if (hiddenObjectDropZone != null)
            ShowExactZoom(hiddenObjectDropZone.transform.position);
    }

    public int GetCostByCategory(HintCategory category)
    {
        return category switch
        {
            HintCategory.LARGE_ZONE => largeZoneCost,
            HintCategory.SMALL_ZONE => smallZoneCost,
            HintCategory.EXACT_ZONE => exactZoneCost,
            _ => 1,
        };
    }


    private bool TryDeductCost(HintCategory hintCategory)
    {
        int cost = GetCostByCategory(hintCategory);

        if (remainingHintCount < cost)
            return false;

        remainingHintCount -= cost;
        OnHintCountUpdated?.Invoke(remainingHintCount);
        return true;
    }

    private bool TryGetHintTarget(string groupID, out Vector3 worldPosition)
    {
        HiddenObjectItem item = HiddenObjectManager.Instance.GetNextUnfoundHiddenObjectByGroupID(groupID);
        if (item != null)
        {
            worldPosition = item.transform.position;
            return true;
        }

        HiddenObjectDropZone dropZone = HiddenObjectManager.Instance.GetNextDropZoneNotPlacedByGroupID(groupID);
        if (dropZone != null)
        {
            worldPosition = dropZone.transform.position;
            return true;
        }

        worldPosition = Vector3.zero;
        return false;
    }

    private void ShowZoneCircle(Vector3 targetWorldPos, float radius, float maxOffset, float cameraZoom)
    {
        float safeMaxOffset = Mathf.Min(maxOffset, radius * 0.6f);
        Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * safeMaxOffset;
        Vector3 circleCenter = new Vector3(
            targetWorldPos.x + randomOffset.x,
            targetWorldPos.y + randomOffset.y,
            targetWorldPos.z);

        Vector3 camTarget = new Vector3(circleCenter.x, circleCenter.y, cameraMover.transform.position.z);
        cameraMover.MoveAndZoomToSameTime(cameraMovementDuration, cameraMovementDuration, cameraZoom, camTarget);

        if (hintCircleRenderer != null)
            hintCircleRenderer.Show(circleCenter, radius);
    }

    private void ShowExactZoom(Vector3 targetWorldPos)
    {
        Vector3 camTarget = new Vector3(targetWorldPos.x, targetWorldPos.y, cameraMover.transform.position.z);
        cameraMover.MoveAndZoomToSameTime(cameraMovementDuration, cameraMovementDuration, cameraExactZoneZoomHint, camTarget);
    }
}
