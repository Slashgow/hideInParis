using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UILevel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private LevelData levelData;

    public static event Action<LevelData> OnTryStartLevel;

    private void StartLevel()
    {
        OnTryStartLevel?.Invoke(levelData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StartLevel();
    }
}
