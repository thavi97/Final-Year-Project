using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainReset : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        collision.transform.position = new Vector3(-0.725f, 2.179662f, -1.876f);
        collision.rigidbody.velocity = Vector3.zero;
    }
}