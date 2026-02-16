using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{

    private SphereCollider triggerCollider;
    [SerializeField] private GameObject zombiePrefabs;
    [SerializeField] private int zombieSpawnCount = 3;
    [SerializeField] private int MaxSpawn = 10; // -1 will be infinity
    [SerializeField] private float spawnDelay = 5f;
    [SerializeField] private bool isActive = false;


    [SerializeField] private float spawnRadius = 4f;

    [SerializeField] private LayerMask tiggerLayer;


    private int currentSpawnedCount = 0;
    private bool isOnCooldown = false;



    private void Awake()
    {
        triggerCollider = GetComponent<SphereCollider>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        bool isPlayer = (tiggerLayer.value & (1 << other.gameObject.layer)) > 0;

        if (isPlayer && isActive && !isOnCooldown)
        {
            // Check spawn limits
            if (MaxSpawn == -1 || currentSpawnedCount < MaxSpawn)
            {
                SpawnZombies();
                StartCoroutine(SpawnCooldown());
            }
        }
    }

    private void SpawnZombies()
    {

        for (int i = 0; i < zombieSpawnCount; i++)
        {
            Vector2 randomPoint = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnOffset = new Vector3(randomPoint.x, 0, randomPoint.y);
            Vector3 spawnPosition = transform.position + spawnOffset;

            NavMeshHit hit;
            // We check within a spawnRadius radius of the random point
            if (NavMesh.SamplePosition(spawnPosition, out hit, spawnRadius, NavMesh.AllAreas))
            {
                Instantiate(zombiePrefabs, hit.position, Quaternion.identity);
                //newZombie.GetComponent<NavMeshAgent>().Warp(hit.position);

                currentSpawnedCount++;
            }

   
        }
        if(currentSpawnedCount == zombieSpawnCount) triggerCollider.enabled = false;
    }

    private IEnumerator SpawnCooldown()
    {
        isOnCooldown = true;
        triggerCollider.enabled = false;
        yield return new WaitForSeconds(spawnDelay);
        triggerCollider.enabled = true;
        isOnCooldown = false;
    }

    public void SetActive(bool status)
    {
        this.isActive = status;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

}
