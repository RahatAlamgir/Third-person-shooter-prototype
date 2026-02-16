using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class ZombieController : MonoBehaviour
{

   
    
    [SerializeField] private float damage = 15f;

    
    private Animator _animator;
    private ZombieCustomizer customizer;
    private ItemDropper dropper;
    
    //private NavMeshAgent agent;

    [SerializeField] private ZombieAttackTrigger handTrigger;

    private enum zombieState { Idle, Walk ,Chasing, Attack, Dead}
    [SerializeField] private zombieState _state = zombieState.Idle;


    
    private int _animIDAttack;
    private int _animIDDead;

    //private bool isDead = false;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        customizer = GetComponent<ZombieCustomizer>();
        dropper = GetComponent<ItemDropper>();
        
    }

    private void Start()
    {
        AssignAnimationIDs();
        
        handTrigger.Setup(damage);
        handTrigger.DisableAttack();

        customizer.SetRandomZombie();

    }


    private void AssignAnimationIDs()
    {
        
        _animIDAttack = Animator.StringToHash("Attack");
        _animIDDead = Animator.StringToHash("Dead");
    }
    public void StartAttack(bool attack)
    {
        //StartCoroutine(DoAttack(delay));
        _animator.SetBool(_animIDAttack, attack);
    }

    public bool IsAttacking()
    {
        // 0 is the base layer of your Animator
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        // AND if the animation progress is less than 100% (1.0f)
        return stateInfo.IsName("ZombieAttack0") && stateInfo.normalizedTime < 1.0f;
    }


    public void StartDamage() => handTrigger.EnableAttack();
    public void EndDamage() => handTrigger.DisableAttack();


    

    public void SetDeath()
    {
        _state = zombieState.Dead; 
        if(dropper!=null) dropper.DropLoot();

        int deadType = UnityEngine.Random.Range(1, 5);
        _animator.SetInteger(_animIDDead, deadType);

        
    }



}
