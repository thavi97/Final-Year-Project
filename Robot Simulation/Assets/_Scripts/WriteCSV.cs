using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class WriteCSV : MonoBehaviour {
    public GameObject grip;
    private string path;
    private string initial;
    StringBuilder csv = new StringBuilder();
    //before your loop

    void Start()
    {
        grip.GetComponent<GameObject>();
        path = "CSV/MyTest_" + DateTime.Now.Millisecond + ".csv";
        initial = "x,y\n";
        File.AppendAllText(path, initial);
    }

    void Update()
    {
        // Create a file to write to.
        float x = grip.transform.position.x;
        float y = grip.transform.position.y;
        string createText = x + "," + y + "\n";
        File.AppendAllText(path, createText);
    }

}
