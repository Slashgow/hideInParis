using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;

public class UIHiddenItem : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Image iconHiddenItem;
    [SerializeField] private Image radialFillBar;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject textParent;

    [Header("Radial Colors")]
    [SerializeField] private Color colorDefault = Color.white;
    [SerializeField] private Color colorCompleted = Color.green;

    private HiddenObjectGroupRuntimeState _runtimeState;

    private int _requiredCount;
    private string _groupID;

    private HiddenObjectItem _placementItem = null;
    private bool _placementLocked = false;

    public void Initialize(HiddenObjectGroupRuntimeState state)
    {
        _runtimeState = state;
        _requiredCount = state.RequiredCount;
        _groupID = state.Data.GroupId;

        iconHiddenItem.sprite = state.Data.Outline == null ? state.Data.Sprite : state.Data.Outline;

        if (state.FoundCount > 0)
            iconHiddenItem.sprite = state.Data.Sprite;

        radialFillBar.fillAmount =state.FoundCount / (float)_requiredCount;
        radialFillBar.color = state.IsCompleted ? colorCompleted : colorDefault;

        SetDescription(state); 
        textParent.SetActive(false);

        // Register with manager so it can call GetWorldPosition / RegisterPlacementItem
        //HiddenObjectManager.Instance?.RegisterUI(_groupID, this);

        HiddenObjectManager.OnAnyItemFound += OnAnyItemFound;
        HiddenObjectManager.OnAnyGroupCompleted += OnAnyGroupCompleted;
        HiddenObjectManager.OnAnyItemPlaced += OnAnyItemPlaced;
        LocalizationSettings.SelectedLocaleChanged += LocalizationSettings_SelectedLocaleChanged;
    }

    private void LocalizationSettings_SelectedLocaleChanged(UnityEngine.Localization.Locale obj) => SetDescription(_runtimeState);

    private void SetDescription(HiddenObjectGroupRuntimeState state)
    {
#if UNITY_WEBGL
        state.Data.Description.GetLocalizedStringAsync().Completed += handle =>
        {
            descriptionText.text = handle.Result;
        };
#else
        descriptionText.text = state.Data.Description.GetLocalizedString();
#endif
    }

    private void OnDisable()
    {
        HiddenObjectManager.OnAnyItemFound -= OnAnyItemFound;
        HiddenObjectManager.OnAnyGroupCompleted -= OnAnyGroupCompleted;
        HiddenObjectManager.OnAnyItemPlaced -= OnAnyItemPlaced;
        LocalizationSettings.SelectedLocaleChanged -= LocalizationSettings_SelectedLocaleChanged;
    }

    private void OnAnyItemFound(string groupId, int foundCount)
    {
        if (!IsMyGroup(groupId)) 
            return;

        iconHiddenItem.sprite = _runtimeState.Data.Sprite;
        radialFillBar.fillAmount = foundCount / (float)_requiredCount;
    }

    private void OnAnyGroupCompleted(string groupId)
    {
        if (!IsMyGroup(groupId)) 
            return;

        radialFillBar.fillAmount = 1f;
        radialFillBar.color = colorCompleted;
    }

    private void OnAnyItemPlaced(string groupId)
    {
        if (!IsMyGroup(groupId)) 
            return;

        _placementLocked = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_placementLocked) return;
        if (_placementItem == null) return;
        if (_placementItem.IsPlaced) return;
        if (!_runtimeState.FoundAll()) return;
        _placementItem.StartPlacementFromUI(GetWorldPosition());
    }

    public void RegisterPlacementItem(HiddenObjectItem item)
    {
        _placementItem = item;
    }

    public Vector3 GetWorldPosition() => iconHiddenItem.transform.position;

    private bool IsMyGroup(string id) => _groupID == id;
}