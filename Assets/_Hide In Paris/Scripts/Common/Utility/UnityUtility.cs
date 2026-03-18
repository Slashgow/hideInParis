using UnityEngine;

namespace inkolorgames
{
    public static class UnityUtility
    {
        public static Vector3 GetRandomOffset(Vector3 minOffset, Vector3 maxOffset)
        {
            return new Vector3(Random.Range(minOffset.x, maxOffset.x),
                               Random.Range(minOffset.y, maxOffset.y),
                               Random.Range(minOffset.z, maxOffset.z));
        }

        public static void DestroyAllChildren(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(transform.GetChild(i).gameObject);
            }
        }

    }
}