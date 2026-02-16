using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private float damageMultiplier = 1f;
    private IDamageAble mainController;

    private void Awake()
    {
        // Your ZombieController implements IDamageAble
        mainController = GetComponentInParent<IDamageAble>();
    }

    // This is the function the bullet will call
    public void ExecuteHit(float baseDamage)
    {
        if (mainController == null || mainController.IsDead()) return;

        
        mainController.TakeDamage(baseDamage);
    }

    // Helper to tell the bullet what kind of hit it was (for colors/crit popups)
    public float GetMultiplier() => damageMultiplier;
}