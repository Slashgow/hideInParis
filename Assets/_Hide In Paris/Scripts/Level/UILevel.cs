using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UILevel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button button;
    [SerializeField] private LevelData levelData;

    [Header("Completion")]
    [SerializeField] private Image radialImageProgress;

    [Header("Unlock")]
    [SerializeField] private Image lockImage;
    [SerializeField] private GameObject textParent;
    [SerializeField] private TextMeshProUGUI unlockText;

    public static event Action<LevelData> OnTryStartLevel;

    private void OnEnable() => button.onClick.AddListener(StartLevel);
    private void OnDisable() => button.onClick.RemoveListener(StartLevel);

    private void Start()
    {
        textParent.gameObject.SetActive(false);
        if (!LevelManager.Instance.IsLevelUnlocked(levelData.ItemId))
        {
            lockImage.gameObject.SetActive(true);
            button.interactable = false;
            radialImageProgress.fillAmount = 0f;
        }
        else
        {
            lockImage.gameObject.SetActive(false);
            button.interactable = true;
            radialImageProgress.fillAmount = LevelManager.Instance.GetProgressPercentageForLevel(levelData.ItemId);
        }
    }
    private void StartLevel() => OnTryStartLevel?.Invoke(levelData);
    public void OnPointerExit(PointerEventData eventData)
    {
        textParent.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (LevelManager.Instance.IsLevelUnlocked(levelData.ItemId))
            return;

        textParent.gameObject.SetActive(true);
        unlockText.text = LevelManager.Instance.GetLevelUnlockDescription(levelData.ItemId);
    }
}
