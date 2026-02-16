using System.Collections;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

public class Health : MonoBehaviour,IDamageAble
{
    [SerializeField] private float health = 100;
    [SerializeField] private bool invincible = false;
    private float maxhealth;

    

    [Header("Setting")]
    [SerializeField] [Range(0,1)] private float resistaance = 0;
    [SerializeField][Range(0, 1)] private float buff = 0;
    [SerializeField][Range(0, 1)] private float debuff = 0;

    private enum type { NPC ,Enemy,FriendlyObject, DangerObject}
    [SerializeField] private type objectType= type.Enemy;

    [Header("Visual References")]
    [SerializeField] private List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    [SerializeField] private bool particaleActive = false;
    [SerializeField] private bool playOnWake = false;
    [SerializeField] [Range(0,1)]private float particleActiveThreshold = 0f;

    [Header("Destroy")]
    [SerializeField] private bool destroyOnDeath = false;
    [SerializeField] private float destroyTimeDelay = 5f;

    private int objType = 2;

    private HealthBar healthBar;

    private bool isDead = false;
    

    private void Awake()
    {
        healthBar = GetComponentInChildren<HealthBar>();
    }

    void Start()
    {
        maxhealth = health;
        SelectObjectType();
        if(healthBar != null ) 
            healthBar.SetMaxHealth(health);
        if (particaleActive && playOnWake)
        {
            PlayAllParticles();
        }
    }

    public bool IsDead() => isDead;
    public int ObjectType() => objType;

    public float GetHealth() => health;

    public float GetDamageThreshold()
    {
        return maxhealth * particleActiveThreshold;
    }

    private void SelectObjectType()
    {
        if (objectType == type.NPC) objType = 1;
        else if (objectType == type.Enemy) objType = 2;
        else if (objectType == type.FriendlyObject) objType = 3;
        else objType = 4;
    }
    

    public void TakeDamage(float amount)
    {
        if (isDead || invincible) return;
         

        if (health <= 0)
        {
            isDead = true;
            health = 0;
            if (healthBar != null)
            {
                healthBar.SetHealth(0);
                healthBar.gameObject.SetActive(false);
            }
            if(destroyOnDeath) Destroy(gameObject, destroyTimeDelay);

        } else
        {
            health-= damageCalculation(amount);
            if (health <= 0) health = 0;
            if(healthBar != null) healthBar.SetHealth(health);
            if (health <= GetDamageThreshold() && particaleActive) PlayAllParticles();
            
        }
    }
    private float damageCalculation(float amount)
    {
        amount = amount * (1 + debuff - buff - resistaance);
        return Mathf.Max(0,amount);
    }
    public void SetDebuff(float value, float time)
    {
        StartCoroutine(DebuffTimer(value, time));
    }
    private IEnumerator DebuffTimer(float value, float time)
    {
        debuff = value;
        yield return new WaitForSeconds(time);
        debuff = 0;
    }

    private void PlayAllParticles()
    {
        foreach (ParticleSystem p in particleSystems)
        {
            if(!p.isPlaying)
                p.Play();
        }
    }

    


}
