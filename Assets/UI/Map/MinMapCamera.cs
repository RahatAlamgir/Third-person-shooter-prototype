using UnityEngine;

public class MinMapCamera : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private GameObject player;
    private void LateUpdate()
    {
        transform.position = new Vector3(player.transform.position.x , 40  , player.transform.position.z);
    }
}
