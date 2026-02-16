using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


public class RifleBullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 50f; // Bullets are usually faster
    [SerializeField] private float spread = 0.02f;
    [SerializeField] private float baseDamage = 15;
    [SerializeField] private AnimationCurve damageFalloff;
    
    [SerializeField] private LayerMask ignorLayerMask;
    public GameObject damagePopupPrefab;

    private Vector3 targetPosition;
    [SerializeField] private float maxLifeTime = 2f; // Fallback to deactivate if it flies forever
    private float currentLifeTime;

    [Header("Ricoched Round")]
    [SerializeField] private bool ricochet = true;
    [SerializeField] private int maxBounces = 2; // How many times can it bounce?
    private int currentBounces = 0;
    [SerializeField] [Range(0,1)] private float angleThreshold = 0.40f; // Lower = shallower angle required
    private float angle;

    public void Setup(Vector3 originalTarget)
    {
        // Reset lifetime/bounces whenever bullet is "re-used" from the pool
        currentLifeTime = 0f;
        currentBounces = 0;
        angle = angleThreshold;

        Vector3 baseDirection = (originalTarget - transform.position).normalized;
        Vector2 spreadDeviation = Random.insideUnitCircle * spread;
        Quaternion spreadRotation = Quaternion.Euler(spreadDeviation.x, spreadDeviation.y, 0);

        Vector3 spreadDirection = Quaternion.LookRotation(baseDirection) * spreadRotation * Vector3.forward;

        float distance = Vector3.Distance(transform.position, originalTarget);
        // We set target very far to ensure it doesn't just stop in mid-air
        this.targetPosition = transform.position + (spreadDirection * 200f);

        transform.forward = spreadDirection;
    }

    public void SetBulletSpread(float spread)
    {
        this.spread = spread;
    }
    public float GetCurrentDamage()
    {
        float lifePercent = currentLifeTime / maxLifeTime;

        // Evaluate the curve based on the bullet's age (0.0 to 1.0)
        float multiplier = damageFalloff.Evaluate(lifePercent);

        return baseDamage * multiplier;
    }
    public void SetDamage(float baseDamage)
    {
        this.baseDamage = baseDamage;
    }


    private void Update()
    {
        // 1. Manage Lifetime
        currentLifeTime += Time.deltaTime;
        if (currentLifeTime >= maxLifeTime)
        {
            Deactivate();
            return;
        }

        // 2. Movement and Collision
        float moveDistance = bulletSpeed * Time.deltaTime;
        Vector3 moveDir = transform.forward;

        if (Physics.Raycast(transform.position, moveDir, out RaycastHit hit, moveDistance, ~ignorLayerMask))
        {
            transform.position = hit.point;
            HandleHit(hit);
        }
        else
        {
            transform.position += moveDir * moveDistance;
        }
    }

    private void HandleHit(RaycastHit hit)
    {
        // Still using Instantiate here (Pool these next if stutter persists!)
        float nugeAmount = 0.05f;
        Vector3 spawnPos = hit.point + (hit.normal * nugeAmount);
        float finalDamage = Mathf.Round(GetCurrentDamage());

        Hitbox hitbox = hit.collider.GetComponent<Hitbox>();
        IDamageAble target = null;
        if (hitbox != null)
        {
            // It's a bone hit! Use the multiplier
            finalDamage = Mathf.Round(baseDamage * hitbox.GetMultiplier());
            hitbox.ExecuteHit(finalDamage);

            // Find the target interface for the effects logic below
            target = hit.collider.GetComponentInParent<IDamageAble>();
        }
        else
        {
            // It's a direct hit to a main body or something else
            target = hit.collider.GetComponentInParent<IDamageAble>();
            if (target != null) target.TakeDamage(finalDamage);
        }

        // --- EFFECTS LOGIC ---
        if (target != null)
        {
            // Blood vs Impact
            int objectType = target.ObjectType();
            string effect = (objectType == 1 || objectType == 2) ? "Blood" : "Impact";
            SimplePooler.Instance.SpawnFromPool(effect, spawnPos, Quaternion.LookRotation(hit.normal));

            if (!target.IsDead())
            {
                // Spawn Damage Number
                GameObject popup = SimplePooler.Instance.SpawnFromPool("Popup", spawnPos + Vector3.up * 0.5f, Quaternion.identity);
                if (popup != null)
                {
                    var damagePopup = popup.GetComponentInChildren<DamagePopup>();
                    
                    if (hitbox != null)
                    {
                        damagePopup.SetValue(finalDamage, hitbox.GetMultiplier());
                    } else
                    {
                        damagePopup.SetValue(finalDamage);
                    }
                }
            }
        }
        else
        {
            SimplePooler.Instance.SpawnFromPool("Impact", spawnPos, Quaternion.LookRotation(hit.normal));
        }

        // --- RICOCHET LOGIC ---
        // Calculate how 'head-on' the hit was using Dot Product
        // 0 = Perpendicular (Grazing), 1 = Parallel (Direct Hit)

        float dot = Vector3.Dot(transform.forward, -hit.normal);

        if (currentBounces < maxBounces && dot < angle)
        {
            Ricochet(hit);
            return;
        }

        Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForceAtPosition(transform.forward * 5f, hit.point, ForceMode.Impulse);
        }

        Deactivate();
    }
    private void Ricochet(RaycastHit hit)
    {
        currentBounces++;

        // 1. Calculate the new direction
        Vector3 reflectDir = Vector3.Reflect(transform.forward, hit.normal);

        // 2. Update bullet orientation and position
        transform.forward = reflectDir;
        transform.position = hit.point + (hit.normal * 0.02f); // Nudge out to prevent re-colliding

        // 3. Optional: Reduce speed or damage after a bounce
        //bulletSpeed *= 0.6f;
        angle *= 2f;
    }
    private void Deactivate()
    {
        // INSTEAD OF DESTROY: This puts the bullet back in the Rifle's "pool"
        gameObject.SetActive(false);
    }
}
