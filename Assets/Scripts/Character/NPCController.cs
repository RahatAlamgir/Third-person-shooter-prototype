using UnityEngine;

public class NPCController : MonoBehaviour,IDamageAble
{

    [SerializeField] private Animator animator;

    [SerializeField] private float health = 100f;
    [SerializeField] private HealthBar healthBar;
    private enum state { idle , walk , run , job , dead, dance}
    [SerializeField] private state _state = state.idle;


    private int _animIDDance;
    private int _animIDIdle;
    private int _animIDjob;
    private int _animIDDie;
   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthBar.SetMaxHealth(health);
        AssignAnimationIDs();
        NPCAnimation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(float amount)
    {
        if (health <= 0)
        {
            healthBar.SetHealth(0);
            return;
        }
        health -= amount;
        healthBar.SetHealth(health);
    }

    private void NPCAnimation()
    {
        if (_state == state.idle)
            animator.SetBool(_animIDIdle, true);
        else if (_state == state.dance)
        {
            animator.SetBool(_animIDDance, true);
        }
    }
    private void AssignAnimationIDs()
    {
        _animIDDance = Animator.StringToHash("Dance");
        _animIDIdle = Animator.StringToHash("Idle");
    }
    public int ObjectType() => 1;
    public bool IsDead() => false;
}
