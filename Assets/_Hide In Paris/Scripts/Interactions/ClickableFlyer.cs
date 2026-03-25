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

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool flying = false;
    private Vector2 direction;
    private float timer = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (animator == null)
            Debug.LogWarning($"[ClickableFlyer] '{name}' has no Animator.", this);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (flying) 
            return;

        TriggerFly();
    }

    private void Update()
    {
        if (!flying) 
            return;

        transform.Translate(direction * flySpeed * Time.deltaTime, Space.World);

        timer -= Time.deltaTime;
        if (timer <= 0f)
            gameObject.SetActive(false);
    }

    public void TriggerFly()
    {
        flying    = true;
        timer     = lifetime;
        spriteRenderer.sortingOrder = 200;
        direction = PickDirection();

        if (animator != null)
            animator.SetTrigger(flyTriggerName);

        if (faceDirection && spriteRenderer != null)
            spriteRenderer.flipX = direction.x < 0f;
    }

    private Vector2 PickDirection()
    {
        float angle = Random.Range(minAngle, maxAngle);
        float rad   = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
    }

    public void Fly() 
    {
        if (flying) 
            return;

        TriggerFly();
    }

    public void ResetFlyer()
    {
        flying = false;
        timer  = 0f;

        if (spriteRenderer != null)
            spriteRenderer.flipX = false;

        if (animator != null)
            animator.ResetTrigger(flyTriggerName);
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
