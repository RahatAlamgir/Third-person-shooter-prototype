using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float fuseTime = 3.0f;
    [SerializeField] private float explosionRadius = 5.0f;
    [SerializeField] private float explosionDamage = 50.0f;
    [SerializeField] private float blastForce = 700.0f;
    [SerializeField] private LayerMask damageableLayers;
    [SerializeField] private AnimationCurve damageFalloff;

    [Header("Effects")]
    [SerializeField] private GameObject explosionVFX; // Drag a particle prefab here

    private bool _hasExploded = false;

    void Start()
    {
        // Start the countdown as soon as the grenade is spawned/thrown
        StartCoroutine(FuseRoutine());
    }

    private IEnumerator FuseRoutine()
    {
        yield return new WaitForSeconds(fuseTime);
        Explode();
    }

    private void Explode()
    {
        if (_hasExploded) return;
        _hasExploded = true;

        
        if (explosionVFX != null)
        {
            GameObject fx = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            Destroy(fx, 5f);
        }

        // Find everything in range
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, damageableLayers);
        List<IDamageAble> targetsHit = new List<IDamageAble>();
        foreach (Collider hit in colliders)
        {

            IDamageAble target = hit.GetComponentInParent<IDamageAble>();

            if (target != null && !targetsHit.Contains(target) && !target.IsDead())
            {

                float distance = Vector3.Distance(transform.position, hit.transform.position);

                float damageMultiplier = damageFalloff.Evaluate(distance / explosionRadius);

                float finalDamage = explosionDamage * damageMultiplier;

                float roundedDamage = Mathf.Round(finalDamage);


                targetsHit.Add(target); // Mark as hit

                if (roundedDamage > 0)
                {
                    target.TakeDamage(roundedDamage);
                    // ... spawn popup

                    Vector3 spawnPos = hit.transform.position + Vector3.up * 0.5f;
                    GameObject popup = SimplePooler.Instance.SpawnFromPool("Popup", spawnPos, Quaternion.identity);

                    if (popup != null)
                    {
                        popup.GetComponentInChildren<DamagePopup>().SetValue(roundedDamage);
                    }
                }
                    
            }
            
            // Apply Physics Blast Force
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(blastForce, transform.position, explosionRadius);
            }
        }

        //  Remove the grenade from the scene
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f); // Transparent red
        Gizmos.DrawSphere(transform.position, explosionRadius); // Solid sphere

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius); // Outline
    }
}
