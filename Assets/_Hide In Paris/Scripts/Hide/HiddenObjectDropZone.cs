using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class HiddenObjectDropZone : MonoBehaviour
{
    [SerializeField] private UnityEvent onPlaceItem;
    [SerializeField] private bool showOnPlace;
    [SerializeField, ShowIf("showOnPlace")] private SpriteRenderer spriteRenderer;

    public void PlaceItem(Sprite sprite)
    {
        onPlaceItem?.Invoke();

        if (showOnPlace && sprite != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.sprite = sprite;
        }
    }
}