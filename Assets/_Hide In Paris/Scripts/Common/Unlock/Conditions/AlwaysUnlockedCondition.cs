using System;
[Serializable]
public class AlwaysUnlockedCondition : UnlockCondition
{
    public override bool IsMet() => true;
    public override string GetDescription() => "Always Available";
}
