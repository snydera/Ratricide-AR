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
        // Sync the Y rotation of the model with the camera's Y rotation
        Vector3 currentRotation = transform.rotation.eulerAngles;
        currentRotation.y = camTransform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(currentRotation);
    }
}
