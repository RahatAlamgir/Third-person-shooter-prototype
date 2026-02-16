using System.Collections;
using UnityEngine;
using DG.Tweening;

public class TargetTester : MonoBehaviour, IDamageAble
{

    [SerializeField] private GameObject target;
    [SerializeField] private float health = 50;
    [SerializeField] private float respawnDelay = 4f;
    [SerializeField] private bool respawn = true;

    
    
    

    [SerializeField] private TargetQuestDisplay targetTesterScore;
    [SerializeField] private int score = 1;
    

    

    public void TakeDamage(float amount)
    {
        if (health > 0)
        {
            health -= amount;
        } 
        if (health <= 0)
        {
            if (targetTesterScore != null) targetTesterScore.AddScore(score);
            if(respawn) StartCoroutine(Respawn());
        }
               
    }

    public void SetRespawn(bool respawn)
    {
        this.respawn = respawn;
    }

    private IEnumerator Respawn()
    {
        health = 50;
        target.SetActive(false);
        yield return new WaitForSeconds(respawnDelay);
        target.SetActive(true);
    }
    public int ObjectType() => 4;
    public bool IsDead() => false;
}
