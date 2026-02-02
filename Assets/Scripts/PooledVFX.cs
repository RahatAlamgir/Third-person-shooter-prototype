using UnityEngine;

public class PooledVFX : MonoBehaviour
{
    private ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {
        // Play the particles from the start
        ps.Clear();
        ps.Play();

        // Automatically disable after the duration of the particles
        Invoke(nameof(Deactivate), ps.main.duration);
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        CancelInvoke(); // Safety cleanup
    }
}