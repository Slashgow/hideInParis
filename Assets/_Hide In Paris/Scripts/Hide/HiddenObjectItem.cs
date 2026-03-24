using UnityEngine;
using UnityEngine.EventSystems;

public class HiddenObjectItem : MonoBehaviour, IPointerDownHandler
{
    [Header("Group")]
    public string groupId;

    [Header("Visuals")]
    [SerializeField] private bool highlightOnFound = true;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool changeColorOnFound = false;

    public bool HightlightOnFound => highlightOnFound;

    [Header("Placement")]
    public bool requiresPlacement = false;
    public HiddenObjectDropZone targetDropZone;
    public Sprite sprite;
    public int requiredCountBeforePlacement = 1;

    public bool IsFound => _found;
    public bool IsPlaced => _placed;

    private bool _found = false;
    private bool _placed = false;

    private Vector3 _originalPosition;

    private bool _dragging = false;
    private Vector3 _offset;

    private FlyToUIAnimation _flyAnimation;
    private Collider2D _col;
    private int originalSortOrder;

    // Stored so we can fly back to UI if placement fails
    private Vector3 _uiWorldPosition;

    private void Awake()
    {
        _originalPosition = transform.position;
        _flyAnimation = GetComponent<FlyToUIAnimation>();
        _col = GetComponent<Collider2D>();
        originalSortOrder = spriteRenderer.sortingOrder;

        if (_flyAnimation != null)
            _flyAnimation.OnComplete += OnFlyComplete;
    }

    private void OnDestroy()
    {
        if (_flyAnimation != null)
            _flyAnimation.OnComplete -= OnFlyComplete;
    }

    public void Highlight() => spriteRenderer.sortingOrder = 400;
    public void UnHighlight() => spriteRenderer.sortingOrder = originalSortOrder;
    private void OnFlyComplete() => gameObject.SetActive(false);


    public void OnPointerDown(PointerEventData eventData)
    {
        if (_found || (_flyAnimation != null && _flyAnimation.IsFlying)) 
            return;

        MarkFound();
    }

    public void MarkFound()
    {
        if (_found) 
            return;

        _found = true;
        HiddenObjectManager.Instance?.ReportItemFound(this);

        if(changeColorOnFound && spriteRenderer != null)
            spriteRenderer.color = Color.black;
    }

    public void FlyToUI(Vector3 worldTarget)
    {
        _uiWorldPosition = worldTarget;

        if (_col != null) 
            _col.enabled = false;

        _flyAnimation?.Fly(worldTarget);
    }

    public void StartPlacementFromUI(Vector3 uiWorldPosition)
    {
        if (_placed) 
            return;

        _uiWorldPosition = uiWorldPosition;

        transform.position = uiWorldPosition;
        transform.localScale = Vector3.one;
        gameObject.SetActive(true);

        if (_col != null)
            _col.enabled = true;

        _dragging = true;
        _offset = Vector3.zero;

        _flyAnimation?.Cancel();
    }

    private void LateUpdate()
    {
        if (!_dragging) 
            return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = transform.position.z;
        transform.position = mouseWorld + _offset;

        if (Input.GetMouseButtonUp(0))
        {
            _dragging = false;
            TryPlace();
        }
    }

    private void TryPlace()
    {
        int collected = HiddenObjectManager.Instance?.GetFoundCount(groupId) ?? 0;
        if (collected < requiredCountBeforePlacement)
        {
            FlyBackToUI();
            return;
        }

        if (!IsOverDropZone())
        {
            FlyBackToUI();
            return;
        }

        // Success
        _placed = true;
        targetDropZone.PlaceItem(sprite);
        HiddenObjectManager.Instance?.ReportItemPlaced(this);
        gameObject.SetActive(false);
    }

    private void FlyBackToUI()
    {
        if (_col != null) 
            _col.enabled = false;

        _flyAnimation?.Fly(_uiWorldPosition);
    }

    private bool IsOverDropZone()
    {
        if (targetDropZone == null)
            return false;

        Vector2 mouse2D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mouse2D);
        return hit != null && hit.gameObject == targetDropZone.gameObject;
    }

    public void ResetItem()
    {
        _found = false;
        _placed = false;
        _dragging = false;
        transform.position = _originalPosition;
        transform.localScale = Vector3.one;
        gameObject.SetActive(true);

        _flyAnimation?.Cancel();
        if (_col != null) _col.enabled = true;
    }
}