using System.Collections;
using UnityEngine;

/// <summary>
/// Delays the start of an idle animation by a random amount of time.
/// Prevents all instances of the same creature from animating in sync.
///
/// Standalone — works on any GameObject with an Animator.
/// </summary>
public class RandomIdleDelay : MonoBehaviour
{
    [Header("Delay")]
    [Tooltip("Minimum delay in seconds before the idle animation starts.")]
    public float minDelay = 0f;

    [Tooltip("Maximum delay in seconds before the idle animation starts.")]
    public float maxDelay = 2f;

    // ── Runtime ────────────────────────────────────────────────────────────────

    private Animator _animator;

    // ── Unity ──────────────────────────────────────────────────────────────────

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        if (_animator == null)
        {
            Debug.LogWarning($"[RandomIdleDelay] '{name}' has no Animator.", this);
            return;
        }

        // Keep the Animator paused until the delay is over
        _animator.enabled = false;

        StartCoroutine(StartAfterDelay());
    }

    // ── Coroutine ──────────────────────────────────────────────────────────────

    private IEnumerator StartAfterDelay()
    {
        float delay = Random.Range(minDelay, maxDelay);
        yield return new WaitForSeconds(delay);

        _animator.enabled = true;
    }
}
