using System;
using DG.Tweening;
using UnityEngine;

namespace inkolorgames.effects
{
    public class RotateObject : Effect
    {
        [SerializeField, Range(0f, 10f)] private float rotationDuration = 1f;
        [SerializeField] private Ease easing;
        [SerializeField] private Vector3 rotationAxis = Vector3.up;
        [SerializeField, Range(-360f, 360f)] private float startAngle = 0f;
        [SerializeField, Range(-360f, 360f)] private float endAngle = 180f;
        [SerializeField] private RotateMode rotateMode = RotateMode.Fast;

        private Tween rotationTween;
        public void Rotate()
        {
            rotationTween?.Kill();

            Vector3 currentRotation = transform.localEulerAngles;
            Vector3 normalizedAxis = rotationAxis.normalized;

            Vector3 startOffset = normalizedAxis * startAngle;
            Vector3 endOffset = normalizedAxis * endAngle;

            transform.localEulerAngles = currentRotation + startOffset;

            rotationTween = transform.DOLocalRotate(currentRotation + endOffset, rotationDuration, rotateMode)
                .SetEase(easing)
                .SetRelative(false)
                .SetUpdate(true);
        }

        public override void DoEffect() => Rotate();

        private void OnDisable()
        {
            rotationTween?.Kill();
            transform.rotation = Quaternion.identity;
        }

    }
}