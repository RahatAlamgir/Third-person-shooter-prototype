using UnityEngine;

public class AutoDeactivate : MonoBehaviour
{
    [SerializeField] private float delay = 1f;

    void OnEnable()
    {
        // Cancel previous invokes and set a new one
        CancelInvoke();
        Invoke(nameof(DisableMe), delay);
    }

    void DisableMe()
    {
        gameObject.SetActive(false);
    }
}
