using UnityEngine;
using System;

public class FlyToUIAnimation : MonoBehaviour
{
    [SerializeField] private float duration = 0.5f;

    public bool IsFlying => _flying;

    public event Action OnComplete;

    private bool _flying = false;
    private Vector3 _startPos;
    private Vector3 _target;
    private float _timer;

    public static event Action OnStartFly;
    public static event Action OnEndFly;

    public void Fly(Vector3 worldTarget)
    {
        _startPos = transform.position;
        _target = worldTarget;
        _timer = 0f;
        _flying = true;
        OnStartFly?.Invoke();
    }

    private void Update()
    {
        if (!_flying) return;

        _timer += Time.deltaTime;
        float t = Mathf.Clamp01(_timer / duration);
        float ease = 1f - Mathf.Pow(1f - t, 3f);

        transform.position = Vector3.Lerp(_startPos, _target, ease);
        //transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, ease);

        if (t >= 1f)
        {
            OnEndFly?.Invoke();
            _flying = false;
            transform.localScale = Vector3.one;
            OnComplete?.Invoke();
        }
    }

    public void Cancel()
    {
        if (!_flying) return;
        _flying = false;
        transform.localScale = Vector3.one;
    }
}