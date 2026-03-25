using UnityEngine;

public abstract class UnlockCondition : MonoBehaviour
{
    public abstract bool IsMet();
    public abstract string GetDescription();
}
