using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHintButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI hintCostText;
    [SerializeField] private HintManager.HintCategory hintCategory;

    private string groupID;
    private void Start() => hintCostText.text = HintManager.Instance.GetCostByCategory(hintCategory).ToString();
    public void Initialize(string groupID) => this.groupID = groupID;
    private void OnEnable() => button.onClick.AddListener(ShowHint);
    private void OnDisable() => button.onClick.RemoveListener(ShowHint);
    private void ShowHint() => HintManager.Instance.ShowHint(groupID, hintCategory);
}

