using System.Collections.Generic;
using UnityEngine;

namespace inkolorgames
{
    public class PoolingSystem : MonoBehaviour
    {
        [SerializeField]
        private GameObject prefab;

        [SerializeField, Range(0, 200)]
        private int growPoolSize = 10;

        private Queue<GameObject> availablePrefab = new Queue<GameObject>();

        public int AvailablePrefabCount => availablePrefab.Count;

        private Vector3 spawnPositionOffset = Vector3.zero;
        private const float MAX_SPAWN_OFFSET = 500f;

        private void Awake()
        {
            GrowPool();
        }

        public GameObject GetPrefabFromPool()
        {
            if (availablePrefab.Count == 0)
                GrowPool();

            var instance = availablePrefab.Dequeue();
            instance.SetActive(true);
            return instance;
        }
        public GameObject GetPrefabFromPool(Transform parent)
        {
            if (availablePrefab.Count == 0)
                GrowPool();

            var instance = availablePrefab.Dequeue();
            instance.transform.SetParent(parent);
            instance.SetActive(true);
            return instance;
        }

        public GameObject GetPrefabFromPool(Vector3 position)
        {
            if (availablePrefab.Count == 0)
                GrowPool();

            var instance = availablePrefab.Dequeue();
            instance.transform.position = position;
            instance.SetActive(true);
            return instance;
        }

        public GameObject GetPrefabFromPool(Vector3 position, Transform parent, bool worldPositionStays)
        {
            if (availablePrefab.Count == 0)
                GrowPool();

            var instance = availablePrefab.Dequeue();
            instance.transform.position = position;
            instance.transform.SetParent(parent, worldPositionStays);
            instance.SetActive(true);
            return instance;
        }

        private void GrowPool()
        {
            for (int i = 0; i < growPoolSize; i++)
            {
                var instanceToAdd = Instantiate(prefab, transform.position + spawnPositionOffset, Quaternion.identity);
                AdjustSpawnPositionOffset();
                AddToPool(instanceToAdd);
            }
        }

        private void AdjustSpawnPositionOffset()
        {
            spawnPositionOffset += new Vector3(2f, 0f, 0f);
            if (spawnPositionOffset.x > MAX_SPAWN_OFFSET)
                spawnPositionOffset = new Vector3(0f, 0f, 0f);
        }

        public void AddToPool(GameObject instance)
        {
            instance.SetActive(false);
            instance.transform.SetParent(transform);
            instance.transform.position = transform.position + spawnPositionOffset;
            AdjustSpawnPositionOffset();
            availablePrefab.Enqueue(instance);
        }

        public void AddToPool(GameObject instance, Transform parent)
        {
            instance.SetActive(false);
            instance.transform.SetParent(transform);
            instance.transform.position = transform.position + spawnPositionOffset;
            AdjustSpawnPositionOffset();
            availablePrefab.Enqueue(instance);
        }

    }
}