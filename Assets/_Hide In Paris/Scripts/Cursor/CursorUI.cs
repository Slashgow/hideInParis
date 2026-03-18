using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorUI : MonoBehaviour
{
    [SerializeField] private Sprite defaultSprite, clickSprite, grabSprite;
    [SerializeField] private InputActionReference clickAction;
    [SerializeField] private InputActionReference pointerPositionAction;
    [SerializeField] private InputActionReference grabAction;

    [SerializeField] private RectTransform cursorTransform;
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private Image cursorImage;

    private Camera canvasCamera;

    private void Awake()
    {
        canvasCamera = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera;
    }

    private void OnEnable()
    {
        Cursor.visible = false;
        clickAction.action.started += StartClick;
        clickAction.action.canceled += CancelClick;
        grabAction.action.started += StartGrab;
        grabAction.action.canceled += CancelGrab;
        pointerPositionAction.action.performed += OnPointerPositionChanged;
    }

    private void CancelGrab(InputAction.CallbackContext context) => cursorImage.sprite = defaultSprite;
    private void StartGrab(InputAction.CallbackContext context) => cursorImage.sprite = grabSprite;
    private void CancelClick(InputAction.CallbackContext context) => cursorImage.sprite = defaultSprite;
    private void StartClick(InputAction.CallbackContext context) => cursorImage.sprite = clickSprite;

    private void OnDisable()
    {
        Cursor.visible = true;
        clickAction.action.started -= StartClick;
        clickAction.action.canceled -= CancelClick;
        grabAction.action.started -= StartGrab;
        grabAction.action.canceled -= CancelGrab;
        pointerPositionAction.action.performed -= OnPointerPositionChanged;
    }

    private void OnPointerPositionChanged(InputAction.CallbackContext context)
    {
        if (cursorTransform == null)
            return;

        var mousePosition = context.ReadValue<Vector2>();
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, mousePosition, canvasCamera, out var localPoint))
        {
            cursorTransform.anchoredPosition = localPoint;
        }
    }
}
