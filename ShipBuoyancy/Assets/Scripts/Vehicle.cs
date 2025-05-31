using System.Collections;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [SerializeField] public string name { get; protected set; }
    [SerializeField] public float health { get; protected set; }
    [SerializeField] public float weight { get; protected set; }
    [SerializeField] public int maxAmmo { get; protected set; }
    [SerializeField] public float reloadTime { get; protected set; }

    public Vehicle(string name, float health, float weight, int maxAmmo, float reloadTime)
    {
        this.name = name;
        this.health = health;
        this.weight = weight;
        this.maxAmmo = maxAmmo;
        this.reloadTime = reloadTime;
    }
}

