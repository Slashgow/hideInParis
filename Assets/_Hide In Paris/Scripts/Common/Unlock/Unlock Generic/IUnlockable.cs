using System.Collections.Generic;

public interface IUnlockable
{
    bool IsUnlocked { get; }
    List<UnlockCondition> UnlockConditions { get; }
    void Unlock();
    bool CheckUnlockCondition();
    string GetUnlockDescription();
}
