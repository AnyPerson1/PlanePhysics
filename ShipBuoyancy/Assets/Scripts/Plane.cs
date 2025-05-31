using Unity.VisualScripting;
using UnityEngine;

public class Plane : Vehicle, IAirMoveable, IAttack, IAirPhysic
{
    public float speed;
    public int ammo;
    public float dragX;
    public float dragY;
    public float dragZ;

    public Vector3 dragMultipler;
    public Vector3 velocity;
    public Vector3 localVelocity;
    private Vector3 lastPosition;
    private Vector3 currentPosition;

    private void Start()
    {
        weight = 18500;
        lastPosition = transform.position;
    }

    private void Update()
    {
        // Manuel test için velocity'yi inspector'dan ayarlıyorsun
        // Ama yine de local'e çevirmelisin
        localVelocity = transform.InverseTransformDirection(velocity);
        CalculateDrag();

        // Drag etkisini gerçek pozisyona uygula (test için)
        velocity = transform.TransformDirection(localVelocity);
        transform.position += velocity * Time.deltaTime;
    }

    public void CalculateVelocity()
    {
        currentPosition = transform.position;
        velocity = (currentPosition - lastPosition) / Time.deltaTime;
        lastPosition = currentPosition;
        localVelocity = transform.InverseTransformDirection(velocity);
    }

    public Plane(string name, float health, float weight, int maxAmmo, float reloadTime)
        : base(name, health, weight, maxAmmo, reloadTime) { }

    public void AirMove(float speed, Vector3 currentSpeed)
    {
    }

    public void CalculateDrag()
    {
        float altitude = transform.position.y;
        float airDensity = Mathf.Lerp(1.225f, 0.088f, altitude / 20000f);

        float areaZ = 6f;
        float areaY = 55f;
        float areaX = 10f;  
        float alignmentZ = Mathf.Abs(Vector3.Dot(transform.forward, velocity.normalized) - 1);
        float alignmentY = Mathf.Abs(Vector3.Dot(transform.up * -1, velocity.normalized) - 1);
        float alignmentX = Mathf.Abs(Vector3.Dot(transform.right, velocity.normalized) - 1);

        // Minimum değer ver
        alignmentY = Mathf.Max(alignmentY, 0.0001f);
        alignmentX = Mathf.Max(alignmentX, 0.0001f);
        alignmentZ = Mathf.Max(alignmentZ, 0.0001f);

        dragX = CalculateAxisDrag(localVelocity.x, areaX, dragMultipler.x, alignmentX, airDensity);
        dragY = CalculateAxisDrag(localVelocity.y, areaY, dragMultipler.y, alignmentY, airDensity);
        dragZ = CalculateAxisDrag(localVelocity.z, areaZ, dragMultipler.z, alignmentZ, airDensity);

        Vector3 dragVector = new Vector3(
            localVelocity.x > 0 ? -dragX : dragX,  // X yönüne göre
            localVelocity.y > 0 ? -dragY : dragY,  // Y yönüne göre  
            localVelocity.z > 0 ? -dragZ : dragZ   // Z yönüne göre
        );

        localVelocity += dragVector;
    }

    float CalculateAxisDrag(float axisVelocity, float area, float dragMultiplier, float alignment, float airDensity)
    {
        float velocitySquared = axisVelocity * axisVelocity;
        float dragForce = 0.5f * airDensity * velocitySquared * dragMultiplier * area * alignment;

        float dragAcceleration = dragForce / weight;

        float dragVelocity = dragAcceleration * Time.deltaTime;
        return dragVelocity;
    }

    public void Attack()
    {
    }
}