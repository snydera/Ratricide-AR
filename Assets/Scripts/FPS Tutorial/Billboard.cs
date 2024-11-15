using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera cam;

    private void Awake()
    {
        if (cam == null)
            //cam = FindAnyObjectByType<Camera>();
            //cam = FindFirstObjectByType<Camera>();
            cam = Camera.main;
        
                

        if (cam == null)
            return;

        
    }

    // Update is called once per frame
    void Update()
    {
        


        transform.LookAt(cam.transform);
        transform.Rotate(Vector3.up * 180);

        if (cam.isActiveAndEnabled == true)
        {
            
        }      
    }
}
