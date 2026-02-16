using DG.Tweening;
using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAgent : MonoBehaviour
{
   
    
    [SerializeField] [Range(0,1)] private float updateSpeed = 0.5f;
    [SerializeField] private bool follow = true;
    [SerializeField] private float detectionRange = 10f;
    private float currentfollowDistance = 0;
    private bool isAttacking = false;



    private GameObject target;
    private NavMeshAgent meshAgent;
    private ZombieController controller;
    private Animator _animator;
    private Health health;


    private int _animIDSpeed;

    private void Awake()
    {
        meshAgent = GetComponent<NavMeshAgent>();
        controller = GetComponent<ZombieController>();
        _animator = GetComponent<Animator>();
        health = GetComponent<Health>();
    
    }

    private void Start()
    {
        currentfollowDistance = detectionRange;
        _animIDSpeed = Animator.StringToHash("Speed");

        target = GameObject.FindGameObjectWithTag("Player");
        SetTarget(target);
    }
    
    
    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;

        
        if (follow && target != null)
        {
            StopAllCoroutines(); // Prevent duplicate loops
            StartCoroutine(FollowTarget());
            StartCoroutine(AnimationSyncLoop());
        }
    }

    private IEnumerator FollowTarget()
    {
        // The loop runs as long as the script is active AND follow is true
        while (enabled && follow)
        {
            if (target != null)
            {
                StartFollowTarget();
               
            }
            if (health.IsDead())
            {
                controller.SetDeath();
                transform.DOKill();
                follow = false;
                meshAgent.isStopped = true; // Physically stop the agent
                                            //meshAgent.ResetPath();
                meshAgent.enabled = false;
                yield break; // Exit the coroutine entirely
            }

            yield return new WaitForSeconds(updateSpeed);
        }
    }
    private IEnumerator AnimationSyncLoop()
    {
        while (enabled)
        {
            if (health.IsDead())
            {
                _animator.SetFloat(_animIDSpeed, 0f);
                yield break;
            }
            float currentSpeed = meshAgent.velocity.magnitude;
            _animator.SetFloat(_animIDSpeed, currentSpeed);

            yield return null;
        }

        
    }
    private void StartFollowTarget()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, target.transform.position);

        if (distanceToPlayer <= currentfollowDistance && !controller.IsAttacking())
        {
            if (distanceToPlayer > 1.6f)
            {

                meshAgent.isStopped = false;
                //meshAgent.updateRotation = true;
                meshAgent.SetDestination(target.transform.position);
                currentfollowDistance = detectionRange * 2;
                controller.StartAttack(false);
            }
            else
            {
                meshAgent.isStopped = true;
                isAttacking = true;
                meshAgent.velocity = Vector3.zero;
                controller.StartAttack(true);
            }


            if (meshAgent.velocity.magnitude < 0.01)
            {
                if (!DOTween.IsTweening(transform))
                {
                    transform.DOLookAt(target.transform.position, 0.2f, AxisConstraint.Y).SetEase(Ease.OutQuad);

                }
            }
        }
        else
        {
            if (!meshAgent.isStopped)
            {
                transform.DOKill();
                meshAgent.isStopped = true;
                meshAgent.ResetPath();
                currentfollowDistance = detectionRange;
            }
            
        }
    }
    


}
