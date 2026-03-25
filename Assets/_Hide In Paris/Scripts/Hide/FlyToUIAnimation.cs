using UnityEngine;
using System;

public class FlyToUIAnimation : MonoBehaviour
{
    [SerializeField] private float duration = 0.5f;

    public bool IsFlying => flying;

    public event Action OnComplete;

    private bool flying = false;
    private Vector3 startPos;
    private Vector3 target;
    private float timer;

    public static event Action OnStartFly;
    public static event Action OnEndFly;

    public void Fly(Vector3 worldTarget)
    {
        startPos = transform.position;
        target = worldTarget;
        timer = 0f;
        flying = true;
        OnStartFly?.Invoke();
    }

    private void Update()
    {
        if (!flying) 
            return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);
        float ease = 1f - Mathf.Pow(1f - t, 3f);

        transform.position = Vector3.Lerp(startPos, target, ease);

        if (t >= 1f)
        {
            OnEndFly?.Invoke();
            flying = false;
            transform.localScale = Vector3.one;
            OnComplete?.Invoke();
        }
    }

    public void Cancel()
    {
        if (!flying) 
            return;

        flying = false;
        transform.localScale = Vector3.one;
    }
}