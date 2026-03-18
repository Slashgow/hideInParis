using inkolorgames;

public class MusicManager : BaseMusicManager
{
    protected override void Awake()
    {
        base.Awake();
        PlayMusicAtIndex(0);
    }
}
