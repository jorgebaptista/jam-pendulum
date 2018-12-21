using System.Collections.Generic;
using UnityEngine;

public class PoolManagerScript : MonoBehaviour
{
    private int childIndex = 0;
    private List<GameObject> cacheList = new List<GameObject>();

    public int PreCache(GameObject prefab, int initialAmount = 5)
    {
        if (prefab == null) Debug.LogError("Pool Manager Precache Method called without prefab argument.");

        foreach (GameObject cachedPrefab in cacheList)
        {
            if (prefab == cachedPrefab) return cacheList.IndexOf(cachedPrefab);
        }

        int poolIndex = childIndex++;

        GameObject cache;
        cache = prefab;

        cacheList.Insert(poolIndex, cache);

        new GameObject(prefab.name + " Pool").transform.parent = transform;

        int amountCached = 0;

        if (prefab.activeInHierarchy)
        {
            ++amountCached;

            prefab.SetActive(false);
            prefab.transform.parent = transform.GetChild(poolIndex);
        }

        for (int i = amountCached; i < initialAmount; i++)
        {
            GameObject cachedPrefab = Instantiate(prefab, transform.GetChild(poolIndex));
            cachedPrefab.SetActive(false);
        }

        return poolIndex;
    }

    public GameObject GetCachedPrefab(int poolIndex)
    {
        Transform pool = transform.GetChild(poolIndex);
        GameObject cachedPrefab;

        for (int i = 0; i < pool.childCount; ++i)
        {
            cachedPrefab = pool.GetChild(i).gameObject;

            if (!cachedPrefab.activeInHierarchy)
            {
                return cachedPrefab;
            }
        }

        cachedPrefab = Instantiate(cacheList[poolIndex], pool);

        cachedPrefab.SetActive(false);
        return cachedPrefab;
    }
}
