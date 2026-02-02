using UnityEngine;

public class bulletSpawnPointRotationFix : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Update()
    {
        transform.rotation = Quaternion.identity;
    }
}
