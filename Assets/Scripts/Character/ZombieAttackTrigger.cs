using System.Collections.Generic;
using UnityEngine;

public class ZombieAttackTrigger : MonoBehaviour
{
    private float damage;
    private bool canDamage = false;
    private List<IDamageAble> hitTargets = new List<IDamageAble>();

    public void Setup(float dmg) => damage = dmg;

    // Called by Animation Events
    public void EnableAttack()
    {
        canDamage = true;
        hitTargets.Clear(); // Reset so we don't hit the same player twice in one swing
    }

    public void DisableAttack() => canDamage = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!canDamage) return;

        // Look for the Player
        IDamageAble target = other.GetComponentInParent<IDamageAble>();

        if (target != null && !hitTargets.Contains(target))
        {
            target.TakeDamage(damage);
            hitTargets.Add(target); // Ensure one hit per swing
            Debug.Log($"Physical hit on: {other.name}");
        }
    }
}
