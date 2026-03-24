using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHintButton : MonoBehaviour
{
    [SerializeField] private Button hintButton;
    [SerializeField] private TextMeshProUGUI hintCountText;
    private void OnEnable()
    {
        hintButton.onClick.AddListener(ShowHint);
        HintManager.OnUseHint += HintManager_OnUseHint;
    }

    private void OnDisable()
    {
        hintButton.onClick.RemoveListener(ShowHint);
        HintManager.OnUseHint -= HintManager_OnUseHint;
    }

    private void Start() => UpdateTextCount(HintManager.Instance.RemainingHintCount);
    private void ShowHint() => HintManager.Instance.ShowHint();
    private void HintManager_OnUseHint(int remainingHint)
    {
        UpdateTextCount(remainingHint);

        if(remainingHint <= 0)
            hintButton.interactable = false;
    }
    private void UpdateTextCount(int remainingHint) => hintCountText.text = remainingHint.ToString();
}
