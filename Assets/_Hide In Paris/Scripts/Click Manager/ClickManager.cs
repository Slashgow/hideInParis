using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ClickManager : MonoBehaviour
{
    [SerializeField] private InputActionReference clickAction;

    [SerializeField] private UnityEvent<Vector3> OnClickWorldPosition;

    private Camera mainCamera;

    public static event Action<SurfaceType> OnClickOnSurface;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        clickAction.action.performed += ClickActionPerformed;
    }

    private void OnDisable()
    {
        clickAction.action.performed -= ClickActionPerformed;
    }

    private void ClickActionPerformed(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        OnClickWorldPosition?.Invoke(worldPosition);

        Collider2D hit = Physics2D.OverlapPoint(worldPosition);

        if (hit == null)
        {
            OnClickOnSurface?.Invoke(SurfaceType.DEFAULT);
            return;
        }
            

        if (hit.TryGetComponent(out Surface surface))
        {
            Debug.Log($"click on {surface.SurfaceType}");
            OnClickOnSurface?.Invoke(surface.SurfaceType);
        }
        else
        {
            OnClickOnSurface?.Invoke(SurfaceType.DEFAULT);
        }
    }
}
