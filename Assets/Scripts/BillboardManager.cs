using UnityEngine;
using System.Collections.Generic;

public class BillboardManager : MonoBehaviour
{
    // A static list allows HealthBars to "register" themselves easily
    public static List<Transform> Billboards = new List<Transform>();
    private Transform _camTransform;

    private void Start()
    {
        _camTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        Quaternion targetRotation = _camTransform.rotation;

        // Loop through all active health bars and snap their rotation
        for (int i = 0; i < Billboards.Count; i++)
        {
            if (Billboards[i] != null)
                Billboards[i].rotation = targetRotation;
        }
    }
}