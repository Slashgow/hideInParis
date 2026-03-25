using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    [SerializeField] private Button button;

    private void OnEnable() => button.onClick.AddListener(Quit);
    private void OnDisable() => button.onClick.RemoveListener(Quit);
    private void Quit() => Application.Quit();
}
