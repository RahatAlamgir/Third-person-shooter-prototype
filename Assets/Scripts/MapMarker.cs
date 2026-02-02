using UnityEngine;

public class MapMarker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(transform.parent.position.x, 20, transform.parent.position.z);
    }

    
}
