using UnityEngine;

public class destroy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float liveTime = 1.0f;
    [SerializeField] private Transform VFX;
    [SerializeField] private bool destroyAllow = true;
    void Start()
    {
        if(destroyAllow)
            Destroy(gameObject,liveTime);

    }
    private void OnDestroy()
    {
        if (VFX != null)
        {
            Instantiate(VFX, transform.position, Quaternion.identity);
        }
    }


}
