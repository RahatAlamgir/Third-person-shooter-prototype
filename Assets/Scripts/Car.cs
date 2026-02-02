using UnityEngine;

public class Car : MonoBehaviour, IDamageAble
{
    public void TakeDamage(float amount)
    {
        Debug.Log("Car");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public int ObjectType() => 3;

    public bool IsDead() => false;
}
