using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableFlyer : MonoBehaviour, IPointerDownHandler
{
    [Header("Animation")]
    [Tooltip("Name of the Animator trigger parameter that starts the fly animation.")]
    public string flyTriggerName = "Fly";

    [Header("Movement")]
    [Tooltip("Speed in units per second while flying.")]
    public float flySpeed = 2f;

    [Tooltip("How long (seconds) before the object is deactivated after clicking.")]
    public float lifetime = 2f;

    [Tooltip("Minimum angle for the random fly direction (degrees, 0 = right).\n" +
             "Default range 20–160 keeps the pigeon flying upward-ish, never straight down.")]
    [Range(-180f, 180f)]
    public float minAngle = 20f;

    [Tooltip("Maximum angle for the random fly direction (degrees, 0 = right).")]
    [Range(-180f, 180f)]
    public float maxAngle = 160f;

    [Header("Flip")]
    [Tooltip("If true, flips the sprite on X so the pigeon faces the direction it flies.")]
    public bool faceDirection = true;


    private Animator    _animator;
    private SpriteRenderer _spriteRenderer;
    private bool        _flying = false;
    private Vector2     _direction;
    private float       _timer = 0f;

    private void Awake()
    {
        _animator       = GetComponent<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (_animator == null)
            Debug.LogWarning($"[ClickableFlyer] '{name}' has no Animator.", this);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (_flying) 
            return;

        TriggerFly();
    }

    private void Update()
    {
        if (!_flying) return;

        // Move in the chosen direction
        transform.Translate(_direction * flySpeed * Time.deltaTime, Space.World);

        // Count down and deactivate
        _timer -= Time.deltaTime;
        if (_timer <= 0f)
            gameObject.SetActive(false);
    }

    // ── Core ───────────────────────────────────────────────────────────────────

    public void TriggerFly()
    {
        _flying    = true;
        _timer     = lifetime;
        _spriteRenderer.sortingOrder = 200;
        _direction = PickDirection();

        // Trigger the animation
        if (_animator != null)
            _animator.SetTrigger(flyTriggerName);

        // Flip sprite to face travel direction
        if (faceDirection && _spriteRenderer != null)
            _spriteRenderer.flipX = _direction.x < 0f;
    }

    private Vector2 PickDirection()
    {
        float angle = Random.Range(minAngle, maxAngle);
        float rad   = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
    }

    public void Fly() 
    {
        if (_flying) 
            return;

        TriggerFly();
    }

    public void ResetFlyer()
    {
        _flying = false;
        _timer  = 0f;

        if (_spriteRenderer != null)
            _spriteRenderer.flipX = false;

        if (_animator != null)
            _animator.ResetTrigger(flyTriggerName);
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Draw the allowed angle range as a fan in the scene view
        Vector3 origin = transform.position;
        float radius   = 1.2f;

        UnityEditor.Handles.color = new Color(1f, 0.6f, 0f, 0.25f);
        UnityEditor.Handles.DrawSolidArc(
            origin,
            Vector3.forward,
            DirectionFromAngle(minAngle),
            maxAngle - minAngle,
            radius);

        UnityEditor.Handles.color = new Color(1f, 0.6f, 0f, 0.9f);
        UnityEditor.Handles.DrawWireArc(
            origin,
            Vector3.forward,
            DirectionFromAngle(minAngle),
            maxAngle - minAngle,
            radius);

        // Min and max boundary lines
        Gizmos.color = new Color(1f, 0.6f, 0f, 0.8f);
        Gizmos.DrawRay(origin, DirectionFromAngle(minAngle) * radius);
        Gizmos.DrawRay(origin, DirectionFromAngle(maxAngle) * radius);
    }

    private static Vector3 DirectionFromAngle(float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);
    }

   
#endif
}
