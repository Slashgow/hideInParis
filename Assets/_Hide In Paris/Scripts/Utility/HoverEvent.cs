using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UnityEvent onPointerEnter, onPointerExit;

    public void OnPointerEnter(PointerEventData eventData) => onPointerEnter?.Invoke();
    public void OnPointerExit(PointerEventData eventData) => onPointerExit?.Invoke();
}
