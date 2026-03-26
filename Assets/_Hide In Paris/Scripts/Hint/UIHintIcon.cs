using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHintIcon : MonoBehaviour
{
    //[SerializeField] private Button hintButton;
    [SerializeField] private Image radialReloadImage;
    [SerializeField] private TextMeshProUGUI hintCountText;
    private void OnEnable()
    {
        //hintButton.onClick.AddListener(ShowHint);
        HintManager.OnHintCountUpdated += HintManager_OnUseHint;
        HintManager.OnReloadTimerTick += UpdateRadialReloadImage;
    }

  

    private void OnDisable()
    {
        //hintButton.onClick.RemoveListener(ShowHint);
        HintManager.OnHintCountUpdated -= HintManager_OnUseHint;
    }

    private void Start()
    {
        UpdateTextCount(HintManager.Instance.RemainingHintCount);
        UpdateRadialReloadImage(0f);
    }

    private void ShowHint() => HintManager.Instance.ShowNextRandomHint();
    private void HintManager_OnUseHint(int remainingHint) => UpdateTextCount(remainingHint);
    private void UpdateRadialReloadImage(float percentage) => radialReloadImage.fillAmount = percentage;
    private void UpdateTextCount(int remainingHint) => hintCountText.text = remainingHint.ToString();
}
