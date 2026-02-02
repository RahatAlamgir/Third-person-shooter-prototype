using UnityEngine;

public interface IDamageAble
{
    public void TakeDamage(float amount);

    public int ObjectType();

    public bool IsDead();
}
