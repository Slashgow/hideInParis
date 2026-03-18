using System.Collections.Generic;
using inkolorgames;
using inkolorgames.effects;
using UnityEngine;

public class PoolInstantiatedEffect : Effect
{
    [SerializeField] private List<PoolingSystem> poolSystems;
    [SerializeField, Range(0, 5)] private int numberOfEffectToSpawn = 3;
    [SerializeField] private bool instantiateFirstOnExactPosition = true;
    [SerializeField] private Vector3 spawnMinOffset;
    [SerializeField] private Vector3 spawnMaxOffset;

    public override void DoEffect()
    {
        if (poolSystems == null || poolSystems.Count == 0)
        {
            Debug.LogWarning("No pool systems assigned to PoolInstantiatedEffect!");
            return;
        }

        if (instantiateFirstOnExactPosition)
        {
            var pool = poolSystems[Random.Range(0, poolSystems.Count)];
            var instance = pool.GetPrefabFromPool(transform.position);
            SetupPooledObject(instance, pool);
        }
        else
        {
            var pool = poolSystems[Random.Range(0, poolSystems.Count)];
            var instance = pool.GetPrefabFromPool(transform.position + UnityUtility.GetRandomOffset(spawnMinOffset, spawnMaxOffset));
            SetupPooledObject(instance, pool);
        }

        for (int i = 0; i < numberOfEffectToSpawn - 1; i++)
        {
            var pool = poolSystems[Random.Range(0, poolSystems.Count)];
            var instance = pool.GetPrefabFromPool(transform.position + UnityUtility.GetRandomOffset(spawnMinOffset, spawnMaxOffset));
            SetupPooledObject(instance, pool);
        }
    }

    public void DoEffect(Vector3 position)
    {
        if (poolSystems == null || poolSystems.Count == 0)
        {
            Debug.LogWarning("No pool systems assigned to PoolInstantiatedEffect!");
            return;
        }

        if (instantiateFirstOnExactPosition)
        {
            var pool = poolSystems[Random.Range(0, poolSystems.Count)];
            var instance = pool.GetPrefabFromPool(position);
            SetupPooledObject(instance, pool);
        }
        else
        {
            var pool = poolSystems[Random.Range(0, poolSystems.Count)];
            var instance = pool.GetPrefabFromPool(position + UnityUtility.GetRandomOffset(spawnMinOffset, spawnMaxOffset));
            SetupPooledObject(instance, pool);
        }

        for (int i = 0; i < numberOfEffectToSpawn - 1; i++)
        {
            var pool = poolSystems[Random.Range(0, poolSystems.Count)];
            var instance = pool.GetPrefabFromPool(position + UnityUtility.GetRandomOffset(spawnMinOffset, spawnMaxOffset));
            SetupPooledObject(instance, pool);
        }
    }

    public virtual void DoEffect(Vector3 position, Color color)
    {
        if (poolSystems == null || poolSystems.Count == 0)
        {
            Debug.LogWarning("No pool systems assigned to PoolInstantiatedEffect!");
            return;
        }

        if (instantiateFirstOnExactPosition)
        {
            var pool = poolSystems[Random.Range(0, poolSystems.Count)];
            var instance = pool.GetPrefabFromPool(position);
            SetupPooledObject(instance, pool, color);
        }
        else
        {
            var pool = poolSystems[Random.Range(0, poolSystems.Count)];
            var instance = pool.GetPrefabFromPool(position + UnityUtility.GetRandomOffset(spawnMinOffset, spawnMaxOffset));
            SetupPooledObject(instance, pool, color);
        }

        for (int i = 0; i < numberOfEffectToSpawn - 1; i++)
        {
            var pool = poolSystems[Random.Range(0, poolSystems.Count)];
            var instance = pool.GetPrefabFromPool(position + UnityUtility.GetRandomOffset(spawnMinOffset, spawnMaxOffset));
            SetupPooledObject(instance, pool, color);
        }
    }

    protected virtual void SetupPooledObject(GameObject instance, PoolingSystem pool, Color color = new Color())
    {
        var pooledObject = instance.GetComponent<PooledObject>();
        if (pooledObject != null)
        {
            pooledObject.SetPool(pool);
        }
    }
}
