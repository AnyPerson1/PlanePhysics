using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Plane : Vehicle
{
    public Plane(string name, float health, float weight, int maxAmmo, float reloadTime)
        : base(name, health, weight, maxAmmo, reloadTime) { }


    [Header("Read-Only")]
    [SerializeField] public Vector3 velocity;

    [Header("Computing Values")]
    [SerializeField] float angleForward2Velocity;

    [Header("Gizmos")]
    [SerializeField] float forwardLenght = 5f;
    [SerializeField] float velocityLenght = 4f;

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.yellow;
        Vector3 forwardEnd = transform.position + transform.forward * forwardLenght;
        Gizmos.DrawLine(transform.position, forwardEnd);

        Handles.Label(forwardEnd + Vector3.up * 0.5f, "transform.forward");

        Gizmos.color = Color.red;
        Vector3 velDir = velocity.magnitude > 0f ? velocity.normalized : Vector3.zero;
        Vector3 velocityEnd = transform.position + velDir * velocityLenght;
        Gizmos.DrawLine(transform.position, velocityEnd);


        Handles.Label(velocityEnd + Vector3.up * 0.5f, "velocity");



        Vector2 v1 = new Vector2(transform.forward.x, transform.forward.z);
        Vector2 v2 = new Vector2(velocity.x, velocity.z);

        if (v1 != Vector2.zero && v2 != Vector2.zero)
        {
            angleForward2Velocity = Vector2.SignedAngle(v1, v2);
        }
        else
        {
            angleForward2Velocity = 0f; 
        }

        Handles.Label(transform.position + Vector3.up * 2f,
            "Forward→Velocity Açısı: " + angleForward2Velocity.ToString("F1") + "°");
    }
}
