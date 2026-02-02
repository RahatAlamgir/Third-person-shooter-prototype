using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class SimplePooler : MonoBehaviour
{
    // Static instance so we can call it from anywhere: SimplePooler.Instance
    public static SimplePooler Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;           // e.g., "Bullet", "Impact", "Popup"
        public GameObject prefab;
        public int size;             // How many to pre-warm
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private Dictionary<string, List<RifleBullet>> bulletScriptPool = new Dictionary<string, List<RifleBullet>>();

    void Awake()
    {
        // Prepare slots for 1000 active tweens and 100 sequences
        // This stops the warning and the mid-game memory allocation
        DOTween.SetTweensCapacity(1000, 100);
        Instance = this;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag)) return null;

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        // IF SOMETHING DESTROYED THE OBJECT MANUALLY:
        if (objectToSpawn == null)
        {
            Debug.LogWarning($"Object in pool '{tag}' was destroyed! Creating a replacement.");
            Pool pool = pools.Find(p => p.tag == tag);
            objectToSpawn = Instantiate(pool.prefab);
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(objectToSpawn);
        return objectToSpawn;
    }
    public RifleBullet GetPooledBullet(Vector3 pos, Quaternion rot)
    {
        GameObject obj = SpawnFromPool("Bullet", pos, rot);
        // You can use a dictionary to store these scripts so you don't call GetComponent
        return obj.GetComponent<RifleBullet>();
    }
}