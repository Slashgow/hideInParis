using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public enum Axis { X, Y }
    public enum PositionSpace { World, Local }

    [Header("Drag Settings")]
    [Tooltip("Which axis the object can move along.")]
    [SerializeField] private Axis axis = Axis.Y;

    [Tooltip("World: min/max are absolute world positions.\n" +
             "Local: min/max are relative to the parent transform, so the object travels " +
             "the same distance regardless of where it is placed in the scene.")]
    [SerializeField] private PositionSpace space = PositionSpace.Local;

    [Tooltip("Minimum position on the chosen axis (in the selected space).")]
    [SerializeField] private float min = -1f;

    [Tooltip("Maximum position on the chosen axis (in the selected space).")]
    [SerializeField] private float max = 1f;

    [Header("Snap Points (optional)")]
    [Tooltip("If set, the object snaps to the nearest of these normalised positions (0-1) on release. " +
             "Leave empty for free movement.")]
    [Range(0f, 1f)]
    [SerializeField] private float[] snapPoints;

    [Tooltip("Distance (in normalised 0-1 space) within which a snap point attracts the object on release.")]
    [Range(0f, 0.5f)]
    [SerializeField] private float snapThreshold = 0.1f;

    [Header("Events")]
    [Tooltip("Fired every frame while dragging. Passes the current normalised value (0 = min, 1 = max).")]
    [SerializeField] private UnityEvent<float> onValueChanged;

    [Tooltip("Fired when the player releases the object. Passes the final normalised value.")]
    [SerializeField] private UnityEvent<float> onReleased;

    [Tooltip("Fired when the object reaches or snaps to min (normalised 0).")]
    [SerializeField] private UnityEvent onReachedMin;

    [Tooltip("Fired when the object reaches or snaps to max (normalised 1).")]
    [SerializeField] private UnityEvent onReachedMax;


    private bool _isDragging = false;
    private float _dragOffset = 0f;
    private Camera _cam;

    private bool _wasAtMin = false;
    private bool _wasAtMax = false;

    public float NormalisedValue => Mathf.InverseLerp(min, max, GetAxisValue());

    private void Awake()
    {
        _cam = Camera.main;
        ClampPosition();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragging = true;
        _dragOffset = GetAxisValue() - PointerToAxisValue();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isDragging) 
            return;

        _isDragging = false;

        TrySnap();
        onReleased?.Invoke(NormalisedValue);
        CheckEdgeEvents();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) 
            return;

        float target = PointerToAxisValue() + _dragOffset;
        float clamped = Mathf.Clamp(target, min, max);
        SetAxisValue(clamped);

        onValueChanged?.Invoke(NormalisedValue);
        CheckEdgeEvents();
    }

   
    private float GetAxisValue()
    {
        if (space == PositionSpace.World)
        {
            return axis == Axis.X ? transform.position.x : transform.position.y;
        }
        else
        {
            return axis == Axis.X ? transform.localPosition.x : transform.localPosition.y;
        }
    }

    /// <summary>Set the axis value in the configured space, leaving the other axes unchanged.</summary>
    private void SetAxisValue(float value)
    {
        if (space == PositionSpace.World)
        {
            Vector3 pos = transform.position;
            if (axis == Axis.X) pos.x = value; else pos.y = value;
            transform.position = pos;
        }
        else
        {
            Vector3 pos = transform.localPosition;
            if (axis == Axis.X) pos.x = value; else pos.y = value;
            transform.localPosition = pos;
        }
    }

    /// <summary>Convert the current pointer position into the same space and axis as the draggable.</summary>
    private float PointerToAxisValue()
    {
        Vector3 worldPointer = _cam.ScreenToWorldPoint(Input.mousePosition);

        if (space == PositionSpace.World)
        {
            return axis == Axis.X ? worldPointer.x : worldPointer.y;
        }
        else
        {
            // Convert world pointer into parent local space
            Transform parent = transform.parent;
            Vector3 localPointer = parent != null
                ? parent.InverseTransformPoint(worldPointer)
                : worldPointer; // no parent: local == world

            return axis == Axis.X ? localPointer.x : localPointer.y;
        }
    }

    private void ClampPosition()
    {
        SetAxisValue(Mathf.Clamp(GetAxisValue(), min, max));
    }

    // ?? Snap ???????????????????????????????????????????????????????????????????

    private void TrySnap()
    {
        if (snapPoints == null || snapPoints.Length == 0) return;

        float current = NormalisedValue;
        float bestDist = float.MaxValue;
        float bestPoint = current;

        foreach (float point in snapPoints)
        {
            float dist = Mathf.Abs(point - current);
            if (dist < bestDist && dist <= snapThreshold)
            {
                bestDist = dist;
                bestPoint = point;
            }
        }

        SetAxisValue(Mathf.Lerp(min, max, bestPoint));
    }

    // ?? Edge Events ????????????????????????????????????????????????????????????

    private void CheckEdgeEvents()
    {
        bool atMin = Mathf.Approximately(GetAxisValue(), min);
        bool atMax = Mathf.Approximately(GetAxisValue(), max);

        if (atMin && !_wasAtMin) onReachedMin?.Invoke();
        if (atMax && !_wasAtMax) onReachedMax?.Invoke();

        _wasAtMin = atMin;
        _wasAtMax = atMax;
    }

    // ?? Public API ?????????????????????????????????????????????????????????????

    /// <summary>Move to a normalised position (0 = min, 1 = max) instantly.</summary>
    public void SetNormalisedValue(float t)
    {
        SetAxisValue(Mathf.Lerp(min, max, Mathf.Clamp01(t)));
        CheckEdgeEvents();
    }

    /// <summary>Move to a normalised position over time. Call from a Coroutine.</summary>
    public System.Collections.IEnumerator AnimateTo(float targetT, float duration)
    {
        float startVal = GetAxisValue();
        float endVal = Mathf.Lerp(min, max, Mathf.Clamp01(targetT));
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetAxisValue(Mathf.Lerp(startVal, endVal, elapsed / duration));
            onValueChanged?.Invoke(NormalisedValue);
            yield return null;
        }

        SetAxisValue(endVal);
        onValueChanged?.Invoke(NormalisedValue);
        CheckEdgeEvents();
    }

    // ?? Gizmos ?????????????????????????????????????????????????????????????????

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Always draw gizmos in world space
        Vector3 minPos, maxPos;

        if (space == PositionSpace.World)
        {
            minPos = transform.position;
            maxPos = transform.position;

            if (axis == Axis.X) { minPos.x = min; maxPos.x = max; }
            else { minPos.y = min; maxPos.y = max; }
        }
        else
        {
            // Convert local min/max to world space for drawing
            Transform parent = transform.parent;

            Vector3 localMin = transform.localPosition;
            Vector3 localMax = transform.localPosition;

            if (axis == Axis.X) { localMin.x = min; localMax.x = max; }
            else { localMin.y = min; localMax.y = max; }

            minPos = parent != null ? parent.TransformPoint(localMin) : localMin;
            maxPos = parent != null ? parent.TransformPoint(localMax) : localMax;
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(minPos, maxPos);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(minPos, 0.05f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(maxPos, 0.05f);

        if (snapPoints != null)
        {
            Gizmos.color = Color.yellow;
            foreach (float t in snapPoints)
                Gizmos.DrawWireSphere(Vector3.Lerp(minPos, maxPos, t), 0.04f);
        }
    }


#endif
}