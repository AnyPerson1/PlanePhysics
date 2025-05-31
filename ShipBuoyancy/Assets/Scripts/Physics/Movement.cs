using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float currentSpeed = 15f;
    [SerializeField] float interpolation_speed = 1f;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private Vector3 firstPosition;
    private void Start()
    {
        targetPosition = Move(currentSpeed);
    }
    private void Update()
    {
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            targetPosition = Move(currentSpeed);
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
    }
    Vector3 Move(float speed)
    {
        return transform.position + transform.forward * speed;
    }
}
