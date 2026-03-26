using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using inkolorgames;
using UnityEngine;
using UnityEngine.Events;

public class HiddenObjectManager : MonoSingleton<HiddenObjectManager>
{
    [Header("Level")]
    [Tooltip("Unique identifier for this level. Defaults to the scene name if left blank.")]
    [SerializeField] private string levelId;

    [Header("Highlight")]
    [SerializeField] private Transform highlightBackground;
    [SerializeField, Range(0f, 5f)] private float highlightDuration = 2f;

    [Header("Groups")]
    [SerializeField] private List<HiddenObjectGroup> groups = new List<HiddenObjectGroup>();
    public List<HiddenObjectGroup> HiddenObjects => groups;
    private List<HiddenObjectItem> items = new List<HiddenObjectItem>();

    [Header("Global Events")]
    [SerializeField] private UnityEvent onAllGroupsCompletedUnity;
    [SerializeField] private UnityEvent<string> onAnyItemFoundUnity;
    [SerializeField] private UnityEvent<string> onAnyGroupCompletedUnity;

    private Dictionary<string, HiddenObjectGroupRuntimeState> _states;
    private Dictionary<string, int> _itemCountsPerGroup;
    private Dictionary<string, UIHiddenItem> _uiByGroup = new();
    private int _completedGroupCount = 0;

    public Dictionary<string, HiddenObjectGroupRuntimeState> States => _states;

    public static event Action<string, int> OnAnyItemFound;
    public static event Action<string> OnAnyGroupCompleted;
    public static event Action<string> OnAnyItemPlaced;
    public static event Action OnStartHighlightingItem;
    public static event Action OnEndHighlightingItem;

    [SerializeField] private UnityEvent<Vector3> OnAnyItemFoundWithPosition;
    [SerializeField] private UnityEvent<Vector3> OnAnyItemPlacedWithPosition;

    private Coroutine coroutine;
    public string LevelId => string.IsNullOrWhiteSpace(levelId)
    ? UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
    : levelId;

    protected override void Awake()
    {
        highlightBackground.gameObject.SetActive(false);
        BuildStates();
        LoadLevel();
    }

    private void BuildStates()
    {
        _states = new Dictionary<string, HiddenObjectGroupRuntimeState>();
        _itemCountsPerGroup = new Dictionary<string, int>();
        _completedGroupCount = 0;

        var allItems = FindObjectsByType<HiddenObjectItem>(FindObjectsSortMode.None);
        items = allItems.ToList();
        foreach (var item in allItems)
        {
            if (string.IsNullOrEmpty(item.groupId)) 
                continue;

            if (!_itemCountsPerGroup.ContainsKey(item.groupId))
                _itemCountsPerGroup[item.groupId] = 0;

            _itemCountsPerGroup[item.groupId]++;
        }

        foreach (var group in groups)
        {
            if (group == null) 
                continue;

            if (_states.ContainsKey(group.GroupId))
            {
                Debug.LogWarning($"[HiddenObjectManager] Duplicate groupId '{group.GroupId}'.");
                continue;
            }

            _itemCountsPerGroup.TryGetValue(group.GroupId, out int autoCount);
            _states[group.GroupId] = new HiddenObjectGroupRuntimeState(group, autoCount);
        }
    }

    public void RegisterUI(string groupId, UIHiddenItem ui)
    {
        _uiByGroup[groupId] = ui;

        if (_states.TryGetValue(groupId, out var state))
            ui.Initialize(state);

        foreach (var item in items)
        {
            if (item.groupId != groupId) 
                continue;
            if (item.requiresPlacement && item.IsFound && !item.IsPlaced)
                ui.RegisterPlacementItem(item);
        }
    }

    public UIHiddenItem GetUI(string groupId) => _uiByGroup.TryGetValue(groupId, out var ui) ? ui : null;


    /// <summary>
    /// Snapshots the current found/placed state of every item and persists it.
    /// Called automatically after each find/place.
    /// </summary>
    public void SaveLevel()
    {
        InsideLevelSaveData data = GameSaveSystem.Instance.LoadInsideLevelData(LevelId);

        data.hiddenObjectStates = items.Select(item => item.GetSaveData()).ToList();

        GameSaveSystem.Instance.SaveInsideLevelData(data);
    }

    /// <summary>
    /// Restores item states silently (no animations, no manager events).
    /// Call once after all UI has been registered, before gameplay starts.
    /// </summary>
    public void LoadLevel()
    {
        InsideLevelSaveData data = GameSaveSystem.Instance.LoadInsideLevelData(LevelId);

        if (data.hiddenObjectStates == null || data.hiddenObjectStates.Count == 0)
            return;

        Dictionary<string, HiddenObjectItemSaveData> lookup = data.hiddenObjectStates.ToDictionary(s => s.sceneKey, s => s);

        foreach (var item in items)
        {
            if (!lookup.TryGetValue(item.SceneKey, out var savedState))
                continue;

            item.RestoreFromSave(savedState);

            if (!_states.TryGetValue(item.groupId, out var groupState))
                continue;

            if (savedState.found)
            {
                groupState.FoundCount++;

                if (!item.requiresPlacement)
                    SilentlyCompleteGroupIfDone(groupState);
            }

            if (savedState.placed)
                SilentlyCompleteGroupIfDone(groupState);
        }
    }

    private void SilentlyCompleteGroupIfDone(HiddenObjectGroupRuntimeState state)
    {
        if (state.IsCompleted) return;
        if (state.FoundCount < state.RequiredCount) return;

        state.IsCompleted = true;
        _completedGroupCount++;
    }



    public void ReportItemFound(HiddenObjectItem item)
    {
        if (!_states.TryGetValue(item.groupId, out var state))
        {
            Debug.LogWarning($"[HiddenObjectManager] No group found for id '{item.groupId}'.");
            return;
        }

        if (state.IsCompleted) 
            return;

        state.FoundCount++;

        // Trigger fly toward UI icon
        if (_uiByGroup.TryGetValue(item.groupId, out var ui))
        {
            //item.FlyToUI(ui.GetWorldPosition());

            if (item.requiresPlacement)
                ui.RegisterPlacementItem(item);
        }

        if(item.HightlightOnFound)
            Highlight(item);

        onAnyItemFoundUnity?.Invoke(item.groupId);
        OnAnyItemFound?.Invoke(item.groupId, state.FoundCount);
        OnAnyItemFoundWithPosition?.Invoke(item.transform.position);

        // Non-placement groups complete on collection
        if (!item.requiresPlacement)
            CheckGroupCompletion(item.groupId, state);

        SaveLevel();
    }

    private void Highlight(HiddenObjectItem item)
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
            highlightBackground.gameObject.SetActive(false);
            OnEndHighlightingItem?.Invoke();
        }

        OnStartHighlightingItem?.Invoke();
        highlightBackground.transform.position = item.transform.position;
        highlightBackground.gameObject.SetActive(true);
        item.Highlight();

        coroutine = StartCoroutine(StopHighlight(item));
    }

    private IEnumerator StopHighlight(HiddenObjectItem item)
    {
        yield return new WaitForSeconds(highlightDuration);
        item.UnHighlight();
        highlightBackground.gameObject.SetActive(false);
        OnEndHighlightingItem?.Invoke();

        if (_uiByGroup.TryGetValue(item.groupId, out var ui))
        {
            item.FlyToUI(ui.GetWorldPosition());
        }
    }

    public void ReportItemPlaced(HiddenObjectItem item)
    {
        if (!_states.TryGetValue(item.groupId, out var state)) return;

        OnAnyItemPlaced?.Invoke(item.groupId);
        OnAnyItemPlacedWithPosition?.Invoke(item.transform.position);
        CheckGroupCompletion(item.groupId, state);

        SaveLevel();
    }

    public int GetFoundCount(string groupId) => _states.TryGetValue(groupId, out var state) ? state.FoundCount : 0;
    public HiddenObjectGroupRuntimeState GetState(string groupId) => _states.TryGetValue(groupId, out var s) ? s : null;
    public float OverallProgress() => _states.Count == 0 ? 1f : (float)_completedGroupCount / _states.Count;
    public int CompletedGroupCount() => _completedGroupCount;
    public int TotalGroupCount() => _states.Count;
    public IEnumerable<HiddenObjectGroupRuntimeState> GetAllStates() => _states.Values;

    private void CheckGroupCompletion(string groupId, HiddenObjectGroupRuntimeState state)
    {
        if (state.IsCompleted) return;
        if (state.FoundCount < state.RequiredCount) return;

        state.IsCompleted = true;
        _completedGroupCount++;

        onAnyGroupCompletedUnity?.Invoke(groupId);
        OnAnyGroupCompleted?.Invoke(groupId);

        if (_completedGroupCount >= _states.Count)
            onAllGroupsCompletedUnity?.Invoke();
    }

    public HiddenObjectItem GetNextUnfoundHiddenObject() => items.FirstOrDefault(item => item.IsFound == false);
    public HiddenObjectDropZone GetNextDropZoneItemNotPlaced() => items.FirstOrDefault(item => item.requiresPlacement && item.IsFound && !item.IsPlaced).targetDropZone;

    public HiddenObjectItem GetNextUnfoundHiddenObjectByGroupID(string groupID) => items.FirstOrDefault(item => item.groupId == groupID && !item.IsFound);

    public HiddenObjectDropZone GetNextDropZoneNotPlacedByGroupID(string groupID) =>
        items.FirstOrDefault(item =>
                item.groupId == groupID &&
                item.requiresPlacement &&
                item.IsFound &&
                !item.IsPlaced)
            ?.targetDropZone;


    [ContextMenu("Log Status")]
    private void LogStatus()
    {
        Debug.Log($"[HiddenObjectManager] {_completedGroupCount}/{_states.Count} groups complete ({OverallProgress():P0})");
        foreach (var state in _states.Values)
            Debug.Log($"  [{(state.IsCompleted ? "✓" : " ")}] {state.Data.DisplayName} — {state.ProgressString()}");
    }
}