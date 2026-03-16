using UnityEngine;

[CreateAssetMenu(fileName = "NewObjectGroup", menuName = "Hidden Object/Group Definition")]
public class HiddenObjectGroup : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("Unique ID. Must match the groupId field on every HiddenObjectItem in this group.")]
    [SerializeField] private string groupId;

    [Tooltip("Display name shown in the UI / checklist (e.g. 'Find all the pigeons').")]
    [SerializeField] private string displayName;

    [Header("Completion")]
    [Tooltip("How many items must be found for this group to count as complete.")]
    [SerializeField] private int requiredCount = 1;

    public string GroupId => groupId;
    public string DisplayName => displayName;
    public int RequiredCount => requiredCount;
}
