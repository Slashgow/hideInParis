using System.Collections.Generic;
using inkolorgames;
using UnityEngine;
using UnityEngine.Events;

public class HiddenObjectManager : MonoSingleton<HiddenObjectManager>
{
    [Header("Groups")]
    [Tooltip("All HiddenObjectGroup ScriptableObjects in this level.")]
    public List<HiddenObjectGroup> groups;

    [Header("Global Events")]
    [Tooltip("Fired when every group is completed (the player wins).")]
    public UnityEvent onAllGroupsCompleted;

    [Tooltip("Fired whenever any item is found. Passes the item's groupId.")]
    public UnityEvent<string> onAnyItemFound;

    [Tooltip("Fired whenever any group is completed. Passes the group's groupId.")]
    public UnityEvent<string> onAnyGroupCompleted;

    private Dictionary<string, GroupRuntimeState> _states;
    private int _completedGroupCount = 0;

    protected override void Awake()
    {
        BuildStates();
    }

    private void BuildStates()
    {
        _states = new Dictionary<string, GroupRuntimeState>();
        _completedGroupCount = 0;

        foreach (var group in groups)
        {
            if (group == null) continue;
            if (_states.ContainsKey(group.GroupId))
            {
                Debug.LogWarning($"[HiddenObjectManager] Duplicate groupId '{group.GroupId}'. Only the first will be used.");
                continue;
            }
            _states[group.GroupId] = new GroupRuntimeState(group);
        }
    }

    public void ReportItemFound(HiddenObjectItem item)
    {
        if (!_states.TryGetValue(item.groupId, out var state))
        {
            Debug.LogWarning($"[HiddenObjectManager] No group found for id '{item.groupId}' (object: '{item.name}').");
            return;
        }

        if (state.IsCompleted) return;

        state.FoundCount++;
        onAnyItemFound?.Invoke(item.groupId);

        if (state.FoundCount >= state.Data.RequiredCount)
        {
            state.IsCompleted = true;
            _completedGroupCount++;
            onAnyGroupCompleted?.Invoke(state.Data.GroupId);

            if (_completedGroupCount >= _states.Count)
                onAllGroupsCompleted?.Invoke();
        }
    }

    public GroupRuntimeState GetState(string groupId) => _states.TryGetValue(groupId, out var s) ? s : null;
    public float OverallProgress() =>_states.Count == 0 ? 1f : (float)_completedGroupCount / _states.Count;
    public int CompletedGroupCount() => _completedGroupCount;
    public int TotalGroupCount() => _states.Count;
    public IEnumerable<GroupRuntimeState> GetAllStates() => _states.Values;


    [ContextMenu("Log Status")]
    private void LogStatus()
    {
        Debug.Log($"[HiddenObjectManager] Overall: {_completedGroupCount}/{_states.Count} groups complete ({OverallProgress():P0})");
        foreach (var state in _states.Values)
        {
            Debug.Log($"  [{(state.IsCompleted ? "✓" : " ")}] {state.Data.DisplayName} — {state.ProgressString()} (needs {state.Data.RequiredCount})");
        }
    }
}
