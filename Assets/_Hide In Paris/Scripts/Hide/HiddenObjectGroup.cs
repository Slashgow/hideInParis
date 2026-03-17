using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Localization;


[CreateAssetMenu(fileName = "NewObjectGroup", menuName = "Hidden Object/Group Definition")]
public class HiddenObjectGroup : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("Unique ID. Must match the groupId field on every HiddenObjectItem in this group.")]
    [SerializeField] private string groupId;
    [SerializeField] private string displayName;
    [SerializeField] private LocalizedString description;
    [SerializeField, ShowAssetPreview ] private Sprite sprite;
    [SerializeField, ShowAssetPreview] private Sprite outline;

    public string GroupId => groupId;
    public string DisplayName => displayName;
    public Sprite Sprite => sprite;
    public LocalizedString Description => description;
    public Sprite Outline => outline;
}
