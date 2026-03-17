public class HiddenObjectGroupRuntimeState
{
    public readonly HiddenObjectGroup Data;
    public int FoundCount = 0;
    public bool IsCompleted = false;
    public readonly int RequiredCount;  

    public HiddenObjectGroupRuntimeState(HiddenObjectGroup data, int requiredCount)
    {
        Data = data;
        RequiredCount = requiredCount;
    }

    public string ProgressString()
    {
        return $"{FoundCount} / {RequiredCount}";
    }

    public bool FoundAll() => FoundCount == RequiredCount;
}
