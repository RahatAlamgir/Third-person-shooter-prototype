using StarterAssets;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private float fireRate = 0.1f;
    [SerializeField] private float bulletSpread = 0.05f;
    
    [SerializeField] private int magazineSize = 60;
    [SerializeField] private int bulletAmount;

    [SerializeField] private float firstShotSpread = 0.1f;
    [SerializeField] private float resetDelay = 0.2f; // Time to wait for spread to reset

    private float lastShootTime;

    [Header("References")]
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private Transform shellSpawnPoint;
    [SerializeField] private AudioSource shootingAudioSource;
    [SerializeField] private StarterAssetsInputs input;

    [Header("ScriptableObject")]
    [SerializeField] private PlayerInventoryData playerInventoryData;

    

    public void RifleShoot(Vector3 targetPosition)
    {

        if (Time.time >= lastShootTime + fireRate && !IsMagazineEmpty())
        {
            // --- SPREAD LOGIC ---
            float currentSpread;

            // If it has been more than 0.2s since the last shot, use the precise spread
            if (Time.time > lastShootTime + resetDelay)
            {
                currentSpread = firstShotSpread;
            }
            else
            {
                // Use normal logic for continuous fire
                currentSpread = bulletSpread;
                if (input.aim) currentSpread /= 2;
                
            }

            // --- SHOOTING LOGIC ---
            Vector3 aimDir = (targetPosition - bulletSpawnPoint.position).normalized;

            GameObject bulletObj = SimplePooler.Instance.SpawnFromPool("Bullet", bulletSpawnPoint.position, Quaternion.LookRotation(aimDir, Vector3.up));

            if (bulletObj.TryGetComponent(out RifleBullet bulletScript))
            {
                bulletScript.SetBulletSpread(currentSpread); // Use our calculated currentSpread
                bulletScript.Setup(targetPosition);
            }
            //bullet Shell
            //SimplePooler.Instance.SpawnFromPool("bulletShell",shellSpawnPoint.position,Quaternion.LookRotation(aimDir,Vector3.right));

            // 1. Spawn the shell and store the reference
            GameObject shell = SimplePooler.Instance.SpawnFromPool("bulletShell", shellSpawnPoint.position, shellSpawnPoint.rotation);

            // 2. Get the Rigidbody component
            Rigidbody rb = shell.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Reset velocity (important when reusing objects from a pool!)
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                // 3. Apply force to the right (relative to the spawn point)
                float ejectForce = 5f;
                rb.AddForce(shellSpawnPoint.right * ejectForce, ForceMode.Impulse);

                // Optional: Add some random torque so it spins while flying
                rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);
            }
            // VFX and Audio
            SimplePooler.Instance.SpawnFromPool("ShootVFX", bulletSpawnPoint.position, Quaternion.LookRotation(aimDir, Vector3.up) * Quaternion.Euler(0, -90, 0));
            SimplePooler.Instance.SpawnFromPool("FlashLight", bulletSpawnPoint.position, Quaternion.identity);

            UseBullet(1);
            PlayShootSound();

            lastShootTime = Time.time; // This updates the timer for the next check
        }
    }

    private void PlayShootSound()
    {
        if (shootingAudioSource != null)
        {
            shootingAudioSource.pitch = Random.Range(0.9f, 1.1f);
            shootingAudioSource.PlayOneShot(shootingAudioSource.clip, Random.Range(0.8f, 1.0f));
        }
    }

    private void UseBullet(int amount)
    {
        bulletAmount -= amount;
        if (bulletAmount < 0) bulletAmount = 0;
    }

    public void Reload()
    {
        // Don't reload if the magazine is already full
        if (bulletAmount >= magazineSize) return;

        int amountNeeded = magazineSize - bulletAmount;

        // Ask the ScriptableObject for ammo from our "pockets"
        int ammoReceived = playerInventoryData.ExtractAmmo(amountNeeded);

        // Add what we got into the magazine
        bulletAmount += ammoReceived;
    }


    public void SetBulletAmount(int amount)
    {
        // Logic Fix: Check the 'amount' passed in, not the current 'bulletAmount'
        if (amount <= magazineSize)
        {
            bulletAmount = amount;
        }
        else
        {
            bulletAmount = magazineSize;
        }
    }

    // Simplified your helper functions
    

    public bool isAutoFire() => true;
    public int GetBulletCount() => bulletAmount;
    public void SetMagazineSize(int size) => this.magazineSize = size;
    public int GetMagazineSize() => magazineSize;
    public bool IsMagazineEmpty() => bulletAmount <= 0;
}
