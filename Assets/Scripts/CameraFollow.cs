using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class CameraFollow : MonoBehaviour
{
    private Transform target;
    private Vector3 cameraOffset;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        cameraOffset = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + cameraOffset, 0.75f);
    }
}