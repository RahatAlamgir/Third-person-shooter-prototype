using System.Collections;
using UnityEngine;

public class Money : MonoBehaviour , IDropable
{


    [SerializeField] private int amount = 10;
    [SerializeField] public int rarity = 1;

    [SerializeField] private PlayerInventoryData playerInventoryData;
    [SerializeField] private LayerMask tiggerLayer;
    [SerializeField] private float disableRbDelay = 2f;
    [SerializeField] private float despawnTimer = 20f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // Start the timer as soon as the money is created
        StartCoroutine(DisablePhysicsAfterDelay(disableRbDelay));
        Destroy(gameObject,despawnTimer);
    }

    private IEnumerator DisablePhysicsAfterDelay(float delay)
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(delay);

        if (rb != null)
        {
            // Setting isKinematic to true effectively "turns off" physics movement
            rb.isKinematic = true;

            // Optional: Stop any remaining velocity so it doesn't slowly slide
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            Debug.Log("Money physics disabled and settled.");
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        
        bool isPlayer = (tiggerLayer.value & (1 << other.gameObject.layer)) > 0;

        if (isPlayer)
        {
            // Logic to add money to the player goes here
            Debug.Log($"Picked up ${amount}!");
            playerInventoryData.AddMoney(amount);

            // Destroy the money object so it can't be picked up again
            Destroy(gameObject);
        }
    }

    public int GetRarity()
    {
        return rarity;
    }
}
