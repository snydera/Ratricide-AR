using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera cam;

    // Update is called once per frame
    void Update()
    {
        if (cam == null)
            //cam = FindAnyObjectByType<Camera>();
            cam = FindFirstObjectByType<Camera>();

        if (cam == null )
            return;


        transform.LookAt(cam.transform);
        transform.Rotate(Vector3.up * 180);

        if (cam.isActiveAndEnabled == true)
        {
            
        }      
    }
}
