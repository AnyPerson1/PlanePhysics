using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public List<Plane> planes;

    private void Start()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Plane");
        foreach (GameObject obj in objects)
        {
            planes.Add(obj.GetComponent<Plane>());
        }
    }
}
