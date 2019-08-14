using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{

    public GameObject conveyorBelt;
    public Transform endPoint;
    public float speed;

    void OnTriggerStay(Collider collider)
    {
        collider.transform.position = Vector3.MoveTowards(collider.transform.position, endPoint.position, speed * Time.deltaTime);
    }
}
