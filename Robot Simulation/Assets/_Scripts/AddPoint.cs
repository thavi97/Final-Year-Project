using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddPoint : MonoBehaviour
{
    public Text counter;
    private int pointIterator;

    // Use this for initialization
    void Start()
    {
        counter = counter.GetComponent<Text>();
    }


    void OnCollisionEnter(Collision collision)
    {
        collision.transform.position = new Vector3(-0.725f, 2.179662f, -1.876f);
        collision.rigidbody.velocity = Vector3.zero;
        pointIterator++;
        counter.text = "" + pointIterator;
    }

}
