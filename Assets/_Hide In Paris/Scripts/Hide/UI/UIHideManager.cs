using UnityEngine;


public class UIHideManager : MonoBehaviour
{
    [SerializeField] private Transform prefabParent;
    [SerializeField] private UIHiddenItem hiddenItemPrefab;

    private void Start()
    {
        InitializeUIHiddenItems();
    }

    private void InitializeUIHiddenItems()
    {
        var hiddenObjectsGroups = HiddenObjectManager.Instance.States;

        foreach (var hiddenObjectState in hiddenObjectsGroups)
        {
            UIHiddenItem uIHiddenItem = Instantiate(hiddenItemPrefab, prefabParent);
            uIHiddenItem.Initialize(hiddenObjectState.Value);
        }
    }

}
