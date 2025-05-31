using UnityEngine;

public interface IAirMoveable
{
    public void AirMove(float speed, Vector3 currentVelocity);
}
public interface IAirPhysic
{
    public void CalculateDrag();
    public void CalculateVelocity();
}
public interface IMoveable
{

}

public interface IAttack
{
    public void Attack();
}
