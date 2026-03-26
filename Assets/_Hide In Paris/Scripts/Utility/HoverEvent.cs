using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private UnityEvent onPointerEnter, onPointerExit, onClick;

    public void OnPointerClick(PointerEventData eventData) => onClick?.Invoke();
    public void OnPointerEnter(PointerEventData eventData) => onPointerEnter?.Invoke();
    public void OnPointerExit(PointerEventData eventData) => onPointerExit?.Invoke();
}
