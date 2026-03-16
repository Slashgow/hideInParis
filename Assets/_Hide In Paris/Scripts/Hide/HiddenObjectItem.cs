using UnityEngine;

public class HiddenObjectItem : MonoBehaviour
{
    [Header("Group")]
    [Tooltip("Must match a groupId defined in HiddenObjectManager.")]
    public string groupId;

    private bool _found = false;
    public bool IsFound => _found;

    private void OnMouseUp() => MarkFound();

    public void MarkFound()
    {
        if (_found) 
            return;

        _found = true;
        HiddenObjectManager.Instance?.ReportItemFound(this);
    }

    public void ResetItem() => _found = false;
}
