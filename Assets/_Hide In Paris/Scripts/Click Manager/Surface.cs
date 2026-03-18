using UnityEngine;

public class Surface : MonoBehaviour
{
    [SerializeField] private SurfaceType surfaceType = SurfaceType.DEFAULT;
    public SurfaceType SurfaceType => surfaceType;
}
