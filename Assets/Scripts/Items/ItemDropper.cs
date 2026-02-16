using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    
    [SerializeField] private List<GameObject> items = new List<GameObject>();
    [SerializeField] private int maxDropItem = 3;
    [SerializeField] private float dropPopForce = 0.5f;

    

    // Call this from your Health script when HP <= 0
    public void DropLoot()
    {
        // 1 to 3 items
        int dropCount = Random.Range(1, maxDropItem+1);
        int spawnedCount = 0;

        // We keep trying to drop items until we hit our dropCount 
        // or we run out of items in the list to check.
        List<GameObject> availableItems = new List<GameObject>(items);

        while (spawnedCount < dropCount && availableItems.Count > 0)
        {
            int randomIndex = Random.Range(0, availableItems.Count);
            GameObject prefab = availableItems[randomIndex];
            IDropable dropable = prefab.GetComponent<IDropable>();

            if (dropable != null)
            {
                // Standard Game Math: Higher Rarity = Lower Chance
                float dropChance = (11 - dropable.GetRarity()) * 10f;
                float roll = Random.Range(0f, 100f);

                if (roll <= dropChance)
                {
                    SpawnItem(prefab);
                    spawnedCount++;
                }
            }

            // Remove from the "available" list so we don't check the same prefab twice 
            // in the same death explosion
            availableItems.RemoveAt(randomIndex);
        }
    }
    private void SpawnItem(GameObject prefab)
    {
        GameObject droppedObj = Instantiate(prefab, transform.position + Vector3.up, Quaternion.identity);
        Rigidbody rb = droppedObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 popDir = new Vector3(Random.Range(-dropPopForce, dropPopForce), 1f, Random.Range(-dropPopForce, dropPopForce));
            rb.AddForce(popDir * 5f, ForceMode.Impulse);
        }
    }
}
