using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace inkolorgames.effects
{
    public class ScaleOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private RectTransform rectTransform;

        [SerializeField, Range(0f, 2f)]
        private float endScale;

        [SerializeField, Range(0f, 5f)]
        private float duration = 0.3f;

        [SerializeField]
        private Ease easing;

        [SerializeField] private bool useSelectableInteractability = false;

        private Tween moveTween;
        private Selectable selectable;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            if (useSelectableInteractability)
                selectable = GetComponent<Selectable>();
        }

        public void Scale()
        {
            if (moveTween != null)
                moveTween.Kill();

            moveTween = rectTransform.DOScale(endScale, duration).SetEase(easing).SetUpdate(true);

        }

        public void ResetPosition()
        {
            //Debug.Log("on mouse exit");
            if (moveTween != null)
                moveTween.Kill();

            moveTween = rectTransform.DOScale(1f, duration).SetEase(easing).SetUpdate(true);
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            if (useSelectableInteractability && !selectable.interactable)
                return;

            Scale();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ResetPosition();
        }

        private void OnDestroy()
        {
            moveTween?.Kill();
        }
    }
}