using UnityEngine;

public class AircraftCameraFollow : MonoBehaviour
{
    [Header("Takip Hedefi")]
    public Transform target; // Uçak transform'u

    [Header("Kamera Pozisyon Ayarları")]
    public Vector3 offset = new Vector3(0, 5, -10); // Kameranın uçağa göre pozisyonu
    public float followSpeed = 2f; // Takip hızı
    public float rotationSpeed = 2f; // Dönüş hızı

    [Header("Gelişmiş Ayarlar")]
    public bool useLocalOffset = true; // Offset'i uçağın local space'inde kullan
    public float dampingDistance = 5f; // Bu mesafeden sonra takip yumuşar

    private Vector3 velocity; // SmoothDamp için
    void LateUpdate()
    {
        if (target == null) return;

        UpdateCameraPosition();
        UpdateCameraRotation();
    }

    void UpdateCameraPosition()
    {
        Vector3 desiredPosition;

        if (useLocalOffset)
        {
            // Uçağın local space'inde offset kullan (uçak döndükçe kamera da döner)
            desiredPosition = target.position + target.TransformDirection(offset);
        }
        else
        {
            // World space'de sabit offset
            desiredPosition = target.position + offset;
        }

        // Smooth takip
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            1f / followSpeed
        );
    }

    void UpdateCameraRotation()
    {
        // Kamerayı uçağa doğru çevir
        Vector3 lookDirection = target.position - transform.position;

        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }
}