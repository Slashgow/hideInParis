using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class HintCircleRenderer : MonoBehaviour
{
    [Header("Circle Shape")]
    [SerializeField, Range(16, 128)] private int segments = 64;

    [Header("Display")]
    [SerializeField, Range(0f, 5f)] private float fadeInDuration = 0.3f;
    [SerializeField, Range(0f, 10f)] private float displayDuration = 3f;
    [SerializeField, Range(0f, 5f)] private float fadeOutDuration = 0.5f;

    private LineRenderer lineRenderer;
    private Coroutine displayCoroutine;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = segments;
        SetAlpha(0f);
    }

    public void Show(Vector3 center, float radius)
    {
        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
            displayCoroutine = null;
        }

        DrawCircle(center, radius);
        displayCoroutine = StartCoroutine(DisplayRoutine());
    }

    public void HideImmediate()
    {
        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
            displayCoroutine = null;
        }
        SetAlpha(0f);
    }


    private void DrawCircle(Vector3 center, float radius)
    {
        float angleStep = 2f * Mathf.PI / segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep;
            float x = center.x + radius * Mathf.Cos(angle);
            float y = center.y + radius * Mathf.Sin(angle);
            lineRenderer.SetPosition(i, new Vector3(x, y, center.z));
        }
    }

    private IEnumerator DisplayRoutine()
    {
        yield return StartCoroutine(FadeAlpha(0f, 1f, fadeInDuration));

        yield return new WaitForSeconds(displayDuration);

        yield return StartCoroutine(FadeAlpha(1f, 0f, fadeOutDuration));

        displayCoroutine = null;
    }

    private IEnumerator FadeAlpha(float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            SetAlpha(to);
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(from, to, elapsed / duration));
            yield return null;
        }
        SetAlpha(to);
    }

    private void SetAlpha(float alpha)
    {
        Color c = lineRenderer.startColor;
        c.a = alpha;
        lineRenderer.startColor = c;
        lineRenderer.endColor = c;
    }
}