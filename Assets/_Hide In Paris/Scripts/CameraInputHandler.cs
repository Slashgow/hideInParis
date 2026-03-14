using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class CameraInputHandler : MonoBehaviour
{
    [SerializeField] private InputActionAsset playerInputActionAsset;
    [SerializeField] private InputActionReference dragInputActionReference;
    [SerializeField] private InputActionReference dragStartInputActionReference;
    [SerializeField] private InputActionReference zoomInputActionReference;
    [SerializeField] private InputActionReference recenterInputActionReference;
    [SerializeField] private InputActionReference moveInputActionReference;

    private bool isDragging = false;
    private Vector2 dragInput;
    private float zoomInput;
    private Vector3 dragOrigin;
    private Vector2 moveInput;

    public bool IsDragging => isDragging;
    public Vector2 DragInput => dragInput;
    public Vector3 DragOrigin => dragOrigin;
    public float ZoomInput => zoomInput;
    public Vector2 MoveInput => moveInput;

    public event Action OnRecenterCamera;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();

        dragStartInputActionReference.action.performed += DragStartPerformed;
        dragStartInputActionReference.action.canceled += DragStartCanceled;
        dragInputActionReference.action.performed += DragPerformed;
        zoomInputActionReference.action.performed += ZoomPerformed;
        zoomInputActionReference.action.canceled += ZoomCanceled;
        recenterInputActionReference.action.performed += RecenterPerformed;
        moveInputActionReference.action.performed += MovePerformed; 
        moveInputActionReference.action.canceled += MoveCanceled; 
    }

    public void DragStartPerformed(CallbackContext context)
    {
        isDragging = true;
        dragOrigin = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }
    public void DragStartCanceled(CallbackContext context) => isDragging = false;
    public void DragPerformed(CallbackContext context) => dragInput = context.ReadValue<Vector2>();
    public void ZoomPerformed(CallbackContext context) => zoomInput = context.ReadValue<Vector2>().y;
    public void ZoomCanceled(CallbackContext context) => zoomInput = context.ReadValue<Vector2>().y;
    public void RecenterPerformed(CallbackContext context) => OnRecenterCamera?.Invoke();
    public void MovePerformed(CallbackContext context) => moveInput = context.ReadValue<Vector2>();
    public void MoveCanceled(CallbackContext context) => moveInput = Vector2.zero;

    void OnEnable()
    {
        playerInputActionAsset.Enable();
    }

    void OnDisable()
    {
        dragStartInputActionReference.action.performed -= DragStartPerformed;
        dragStartInputActionReference.action.canceled -= DragStartCanceled;
        dragInputActionReference.action.performed -= DragPerformed;
        zoomInputActionReference.action.performed -= ZoomPerformed;
        zoomInputActionReference.action.canceled -= ZoomCanceled;
        recenterInputActionReference.action.performed -= RecenterPerformed;
        moveInputActionReference.action.performed -= MovePerformed;
        moveInputActionReference.action.canceled -= MoveCanceled;

        playerInputActionAsset.Disable();
    }
}