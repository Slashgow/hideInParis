public class NeverUnlockedCondition : UnlockCondition
{
    public override bool IsMet() => false;
    public override string GetDescription() => "never available";
}