using DG.Tweening;
using inkolorgames.effects;
using UnityEngine;

public class ScaleEffect : Effect
{
    [SerializeField, Range(0f, 1f)] private float duration = 0.2f;
    [SerializeField, Range(0f,3f)] private float endScale = 1.1f;
    [SerializeField, Range(0f,3f)] private float startScale = 1f;
    [SerializeField] private Ease easing;

    public override void DoEffect() => Scale();

    private void Scale()
    {
        this.transform.localScale = Vector3.one * startScale;
        this.transform.DOScale(endScale, duration).SetEase(easing);
    }

}
