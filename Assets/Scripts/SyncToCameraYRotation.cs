using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SyncToCameraYRotation : MonoBehaviour
{
    [SerializeField] Transform camTransform;

    // Update is called once per frame
    void FixedUpdate()
    {
        //transform.localRotation = Quaternion.Euler(0, camTransform.localRotation.y, 0);
        transform.Rotate(0, camTransform.localRotation.y, 0);
    }
}
