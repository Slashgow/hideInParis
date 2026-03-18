using UnityEngine;

public class SetSortOrder : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRendererParent, spriteRenderer;

    private void Awake()
    {
        spriteRenderer.sortingOrder = spriteRendererParent.sortingOrder + 1;
    }
}
