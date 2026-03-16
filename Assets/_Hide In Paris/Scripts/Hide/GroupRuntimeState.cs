public class GroupRuntimeState
{
    public readonly HiddenObjectGroup Data;
    public int FoundCount = 0;
    public bool IsCompleted = false;

    public GroupRuntimeState(HiddenObjectGroup data) => Data = data;

    public string ProgressString()
    {
        int total = Data.RequiredCount;
        return $"{FoundCount} / {total}";
    }
}
