using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GripOpen : MonoBehaviour {

    public Transform gripLeft;
    public Transform gripRight;

    public void gripOpen()
    {
        gripLeft.transform.Translate(Vector3.left * 0.4f);
        gripRight.transform.Translate(Vector3.right * 0.4f);
    }

    public void gripClose()
    {
        gripLeft.transform.Translate(Vector3.right * 0.4f);
        gripRight.transform.Translate(Vector3.left * 0.4f);
    }


}
