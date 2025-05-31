using UnityEngine;

public class AircraftCameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Position Settings")]
    [SerializeField] private Vector3 followOffset = new Vector3(0, 5, -10); // Local offset
    [SerializeField] private float followSpeed = 2f;
    [SerializeField] private float rotationSpeed = 2f;

    [Header("Look Ahead")]
    [SerializeField] private bool useLookAhead = true;
    [SerializeField] private float lookAheadFactor = 3f;
    [SerializeField] private float lookAheadSpeed = 1f;

    [Header("Dynamic Distance")]
    [SerializeField] private bool useSpeedBasedDistance = true;
    [SerializeField] private float minDistance = 8f;
    [SerializeField] private float maxDistance = 25f;
    [SerializeField] private float speedThreshold = 50f; // m/s

    [Header("Banking Effect")]
    [SerializeField] private bool followBanking = true;
    [SerializeField] private float bankingStrength = 0.5f;

    [Header("Look At Settings")]
    [SerializeField] private bool strictLookAt = false; // Tam olarak u�a�a bak
    [SerializeField] private float lookAtHeight = 0f; // U�a��n ne kadar yukar�s�na bak
    [SerializeField] private float maxLookDownAngle = 30f; // Max a�a�� bak�� a��s�

    [Header("Smoothing")]
    [SerializeField] private float positionDamping = 5f;
    [SerializeField] private float rotationDamping = 3f;

    private Vector3 currentVelocity;
    private Vector3 lookAheadPos;
    private Rigidbody targetRigidbody;
    private Vector3 lastTargetPosition;
    private Vector3 targetVelocity;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target not assigned to AircraftCameraFollow!");
            return;
        }

        targetRigidbody = target.GetComponent<Rigidbody>();
        lastTargetPosition = target.position;

        // Initial positioning
        Vector3 desiredPosition = target.position + target.TransformDirection(followOffset);
        transform.position = desiredPosition;
        transform.LookAt(target.position);
    }

    void LateUpdate()
    {
        if (target == null) return;

        CalculateTargetVelocity();
        Vector3 desiredPosition = CalculateDesiredPosition();
        Quaternion desiredRotation = CalculateDesiredRotation();

        // Smooth movement
        transform.position = Vector3.Lerp(transform.position, desiredPosition, positionDamping * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationDamping * Time.deltaTime);
    }

    void CalculateTargetVelocity()
    {
        if (targetRigidbody != null)
        {
            targetVelocity = targetRigidbody.linearVelocity;
        }
        else
        {
            // Manual velocity calculation
            targetVelocity = (target.position - lastTargetPosition) / Time.deltaTime;
            lastTargetPosition = target.position;
        }
    }

    Vector3 CalculateDesiredPosition()
    {
        Vector3 currentOffset = followOffset;

        // Speed-based distance adjustment
        if (useSpeedBasedDistance)
        {
            float speed = targetVelocity.magnitude;
            float speedRatio = Mathf.Clamp01(speed / speedThreshold);
            float dynamicDistance = Mathf.Lerp(minDistance, maxDistance, speedRatio);

            // Adjust Z offset based on speed
            currentOffset.z = -dynamicDistance;
        }

        // Calculate base position
        Vector3 basePosition = target.position + target.TransformDirection(currentOffset);

        // Look ahead positioning
        if (useLookAhead && targetVelocity.magnitude > 1f)
        {
            Vector3 lookAheadTarget = target.position + targetVelocity.normalized * lookAheadFactor;
            lookAheadPos = Vector3.Lerp(lookAheadPos, lookAheadTarget, lookAheadSpeed * Time.deltaTime);

            // Blend between normal follow and look ahead
            Vector3 lookAheadOffset = target.TransformDirection(currentOffset);
            basePosition = Vector3.Lerp(basePosition, lookAheadPos + lookAheadOffset, 0.3f);
        }

        return basePosition;
    }

    Quaternion CalculateDesiredRotation()
    {
        Vector3 lookTarget;

        if (useLookAhead && targetVelocity.magnitude > 5f)
        {
            // Look at where the aircraft is going
            Vector3 futurePosition = target.position + targetVelocity * 0.5f;
            lookTarget = futurePosition;
        }
        else
        {
            // Look at aircraft
            lookTarget = target.position;
        }

        // Look at height adjustment
        lookTarget.y += lookAtHeight;

        Vector3 lookDirection = (lookTarget - transform.position).normalized;

        // Strict look at kontrol�
        if (!strictLookAt)
        {
            // A�a�� bak�� a��s�n� s�n�rla
            float lookDownAngle = Vector3.Angle(Vector3.forward, Vector3.ProjectOnPlane(lookDirection, Vector3.right));
            if (lookDownAngle > maxLookDownAngle)
            {
                // Bak�� a��s�n� s�n�rla
                Vector3 forward = Vector3.ProjectOnPlane(lookDirection, Vector3.up).normalized;
                Vector3 limitedDirection = Quaternion.AngleAxis(-maxLookDownAngle, Vector3.Cross(forward, Vector3.up)) * forward;
                lookDirection = limitedDirection;
            }
        }

        Quaternion baseLookRotation = Quaternion.LookRotation(lookDirection);

        // Banking effect
        if (followBanking)
        {
            float bankAngle = Vector3.Dot(target.right, Vector3.down) * 45f;
            Vector3 bankingEuler = baseLookRotation.eulerAngles;
            bankingEuler.z += bankAngle * bankingStrength;
            return Quaternion.Euler(bankingEuler);
        }

        return baseLookRotation;
    }

    // Cinematic camera shake for combat/turbulence
    public void AddCameraShake(float intensity, float duration)
    {
        StartCoroutine(CameraShake(intensity, duration));
    }

    private System.Collections.IEnumerator CameraShake(float intensity, float duration)
    {
        Vector3 originalOffset = followOffset;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;

            followOffset = originalOffset + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            intensity = Mathf.Lerp(intensity, 0, elapsed / duration);
            yield return null;
        }

        followOffset = originalOffset;
    }

    // Debug visualization
    void OnDrawGizmosSelected()
    {
        if (target == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(target.position + target.TransformDirection(followOffset), 1f);

        if (useLookAhead)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(lookAheadPos, 0.5f);
            Gizmos.DrawLine(target.position, lookAheadPos);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, target.position);
    }
}