using UnityEngine;

public class ZombieController : MonoBehaviour, IDamageAble
{

    [SerializeField] private float health = 100f;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private float damage = 15f;
    [SerializeField] private Animator _animator;



    private enum zombieState { Idle, Walk ,Chasing, Attack, Dead}
    [SerializeField] private zombieState _state = zombieState.Idle;


    private int _animIDSpeed;
    private int _animIDAttack;
    private int _animIDDead;

    //private bool isDead = false;

    private void Start()
    {
        AssignAnimationIDs();
        healthBar.SetMaxHealth(health);
    }

    

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDAttack = Animator.StringToHash("Attack");
        _animIDDead = Animator.StringToHash("Dead");
    }

    public int ObjectType() => 2;
    public bool IsDead() => _state == zombieState.Dead;
    

    public void TakeDamage(float amount)
    {
        if (_state == zombieState.Dead) return;

        health -= amount;
        healthBar.SetHealth(health);

        if (health <= 0)
        {
            Die(); 
        }
    }

    private void Die()
    {
        _state = zombieState.Dead; 

        int deadType = UnityEngine.Random.Range(1, 5);
        _animator.SetInteger(_animIDDead, deadType);

        // Optional: Hide health bar on death
        healthBar.gameObject.SetActive(false);
    }



}
