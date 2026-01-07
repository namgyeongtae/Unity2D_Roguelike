using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private const float MIN_CLAMP_X = -9;
    private const float MAX_CLAMP_X = 32;
    private const float MIN_CLAMP_Y = -8.5f;
    private const float MAX_CLAMP_Y = 6.5f;

    [SerializeField] private Transform followTarget;
    [SerializeField] private Vector3 offset;

    void Start()
    {
        
    }
    void LateUpdate()
    {
        FollowTarget();
    }

    void FollowTarget()
    {
        Vector3 targetPos = followTarget.position + offset;

        targetPos.x = Mathf.Clamp(targetPos.x, MIN_CLAMP_X, MAX_CLAMP_X);
        targetPos.y = Mathf.Clamp(targetPos.y, MIN_CLAMP_Y, MAX_CLAMP_Y);

        transform.position = new Vector3(targetPos.x, targetPos.y, transform.position.z);
    }
}
