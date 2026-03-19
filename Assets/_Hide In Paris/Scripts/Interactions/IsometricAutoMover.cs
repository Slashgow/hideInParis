using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

public class IsometricAutoMover : MonoBehaviour, IPointerClickHandler
{
    public enum IsometricDirection { RightDown, LeftDown }
    public IsometricDirection axisDirection = IsometricDirection.RightDown;
    public float angle = 30f;

    [Header("Motion")]
    public float travelDistance = 2f;
    public float speed = 1f;

    [Header("Boost")]
    public float boostSpeed = 8f;
    public float boostDamping = 5f;

    [SerializeField] private bool reverseOnChangeDirection;
    [SerializeField, ShowIf("reverseOnChangeDirection")] private Transform visual;

    private Vector3 axis;
    private Vector3 origin;
    private float phase;

    private bool isBoosting;
    private Vector3 boostTarget;
    private bool isReverse;

    void Start()
    {
        origin = transform.position;
        phase = Random.Range(0f, Mathf.PI * 2f);
        UpdateAxis();

        if(reverseOnChangeDirection && axisDirection == IsometricDirection.LeftDown)
        {
            Debug.Log("reverse");
            visual.localScale = new Vector3(visual.localScale.x * -1f, visual.localScale.y, visual.localScale.z);
        }
    }

    void UpdateAxis()
    {
        float rad = angle * Mathf.Deg2Rad;
        float xDir = axisDirection == IsometricDirection.RightDown ? 1f : -1f;
        axis = new Vector3(xDir * Mathf.Cos(rad), -Mathf.Sin(rad), 0f).normalized;
    }

    void Update()
    {
        if (isBoosting)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                boostTarget,
                boostSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, boostTarget) < 0.01f)
            {
                transform.position = boostTarget;
                // sync phase to match the reached position
                float t = (boostTarget == origin + axis * travelDistance) ? Mathf.PI * 0.5f : Mathf.PI * 1.5f;
                phase = t;
                isBoosting = false;
            }
        }
        else
        {
            phase += speed * Time.deltaTime;
            transform.position = origin + axis * (Mathf.Sin(phase) * travelDistance);

            if(reverseOnChangeDirection && !isReverse && Mathf.Sin(phase) >= 1f)
            {
                isReverse = true;
                Debug.Log("reverse");
                visual.localScale = new Vector3(visual.localScale.x * -1f, visual.localScale.y, visual.localScale.z);
            }
            else if(reverseOnChangeDirection && isReverse && Mathf.Sin(phase) <= -1f)
            {
                isReverse = false;
                Debug.Log("reverse false");
                visual.localScale = new Vector3(visual.localScale.x * -1f, visual.localScale.y, visual.localScale.z);
            }
        }
    }
    public void OnPointerClick(PointerEventData eventData) => Boost();
    public void Boost()
    {
        UpdateAxis();

        Vector3 posMin = origin - axis * travelDistance;
        Vector3 posMax = origin + axis * travelDistance;

        // pick whichever end is farther from current position
        float distToMax = Vector3.Distance(transform.position, posMax);
        float distToMin = Vector3.Distance(transform.position, posMin);

        boostTarget = distToMax > distToMin ? posMax : posMin;
        isBoosting = true;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Vector3 orig = Application.isPlaying ? origin : transform.position;

        float rad = angle * Mathf.Deg2Rad;
        float xDir = axisDirection == IsometricDirection.RightDown ? 1f : -1f;
        Vector3 ax = new Vector3(xDir * Mathf.Cos(rad), -Mathf.Sin(rad), 0f).normalized;

        Vector3 posMin = orig - ax * travelDistance;
        Vector3 posMax = orig + ax * travelDistance;

        // axis line
        Gizmos.color = new Color(0.4f, 0.8f, 1f, 0.5f);
        Gizmos.DrawLine(posMin, posMax);

        // min marker (red)
        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.9f);
        Gizmos.DrawSphere(posMin, 0.1f);
        UnityEditor.Handles.Label(posMin + Vector3.up * 0.2f, "min");

        // max marker (green)
        Gizmos.color = new Color(0.3f, 1f, 0.4f, 0.9f);
        Gizmos.DrawSphere(posMax, 0.1f);
        UnityEditor.Handles.Label(posMax + Vector3.up * 0.2f, "max");

        // origin marker
        Gizmos.color = new Color(1f, 1f, 0.3f, 0.7f);
        Gizmos.DrawWireSphere(orig, 0.08f);

        // direction arrow
        Gizmos.color = new Color(0.4f, 0.8f, 1f, 0.9f);
        Vector3 arrowTip = orig + ax * (travelDistance * 0.5f);
        Gizmos.DrawLine(orig, arrowTip);
        Gizmos.DrawLine(arrowTip, arrowTip + Quaternion.Euler(0, 0, 150) * ax * 0.2f);
        Gizmos.DrawLine(arrowTip, arrowTip + Quaternion.Euler(0, 0, -150) * ax * 0.2f);

        // boost target preview (editor only)
        if (isBoosting)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, boostTarget);
        }
    }


#endif
}