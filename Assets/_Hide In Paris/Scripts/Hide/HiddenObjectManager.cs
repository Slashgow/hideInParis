using System;
using System.Collections.Generic;
using inkolorgames;
using UnityEngine;
using UnityEngine.Events;

public class HiddenObjectManager : MonoSingleton<HiddenObjectManager>
{
    [Header("Groups")]
    [SerializeField] private List<HiddenObjectGroup> groups = new List<HiddenObjectGroup>();
    public List<HiddenObjectGroup> HiddenObjects => groups;

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

    protected override void Awake()
    {
        BuildStates();
    }

    private void BuildStates()
    {
        _states = new Dictionary<string, HiddenObjectGroupRuntimeState>();
        _itemCountsPerGroup = new Dictionary<string, int>();
        _completedGroupCount = 0;

        var allItems = FindObjectsByType<HiddenObjectItem>(FindObjectsSortMode.None);
        foreach (var item in allItems)
        {
            if (string.IsNullOrEmpty(item.groupId)) continue;

            if (!_itemCountsPerGroup.ContainsKey(item.groupId))
                _itemCountsPerGroup[item.groupId] = 0;

            _itemCountsPerGroup[item.groupId]++;
        }

        foreach (var group in groups)
        {
            if (group == null) continue;

            if (_states.ContainsKey(group.GroupId))
            {
                Debug.LogWarning($"[HiddenObjectManager] Duplicate groupId '{group.GroupId}'.");
                continue;
            }

            _itemCountsPerGroup.TryGetValue(group.GroupId, out int autoCount);
            _states[group.GroupId] = new HiddenObjectGroupRuntimeState(group, autoCount);
        }
    }

    // -------------------------------------------------------
    // UI Registration
    // -------------------------------------------------------

    public void RegisterUI(string groupId, UIHiddenItem ui)
    {
        _uiByGroup[groupId] = ui;
    }

    public UIHiddenItem GetUI(string groupId)
    {
        return _uiByGroup.TryGetValue(groupId, out var ui) ? ui : null;
    }

    // -------------------------------------------------------
    // Reporting
    // -------------------------------------------------------

    public void ReportItemFound(HiddenObjectItem item)
    {
        if (!_states.TryGetValue(item.groupId, out var state))
        {
            Debug.LogWarning($"[HiddenObjectManager] No group found for id '{item.groupId}'.");
            return;
        }

        if (state.IsCompleted) return;

        state.FoundCount++;

        // Trigger fly toward UI icon
        if (_uiByGroup.TryGetValue(item.groupId, out var ui))
        {
            item.FlyToUI(ui.GetWorldPosition());

            if (item.requiresPlacement)
                ui.RegisterPlacementItem(item);
        }

        onAnyItemFoundUnity?.Invoke(item.groupId);
        OnAnyItemFound?.Invoke(item.groupId, state.FoundCount);

        // Non-placement groups complete on collection
        if (!item.requiresPlacement)
            CheckGroupCompletion(item.groupId, state);
    }

    public void ReportItemPlaced(HiddenObjectItem item)
    {
        if (!_states.TryGetValue(item.groupId, out var state)) return;

        OnAnyItemPlaced?.Invoke(item.groupId);
        CheckGroupCompletion(item.groupId, state);
    }

    // -------------------------------------------------------
    // Queries
    // -------------------------------------------------------

    public int GetFoundCount(string groupId)
    {
        return _states.TryGetValue(groupId, out var state) ? state.FoundCount : 0;
    }

    public HiddenObjectGroupRuntimeState GetState(string groupId) =>
        _states.TryGetValue(groupId, out var s) ? s : null;

    public float OverallProgress() =>
        _states.Count == 0 ? 1f : (float)_completedGroupCount / _states.Count;

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

    [ContextMenu("Log Status")]
    private void LogStatus()
    {
        Debug.Log($"[HiddenObjectManager] {_completedGroupCount}/{_states.Count} groups complete ({OverallProgress():P0})");
        foreach (var state in _states.Values)
            Debug.Log($"  [{(state.IsCompleted ? "✓" : " ")}] {state.Data.DisplayName} — {state.ProgressString()}");
    }
}