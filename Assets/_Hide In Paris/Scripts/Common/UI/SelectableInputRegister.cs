using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace inkolorgames.effects
{
    public class SelectableInputRegister : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public static event Action OnAnySelectableHover;
        public static event Action OnAnySelectablePressed;
        public static event Action OnAnySelectableExit;
        public void OnPointerClick(PointerEventData eventData) => OnAnySelectablePressed?.Invoke();
        public void OnPointerEnter(PointerEventData eventData) => OnAnySelectableHover?.Invoke();
        public void OnPointerExit(PointerEventData eventData) => OnAnySelectableExit?.Invoke();
    }
}