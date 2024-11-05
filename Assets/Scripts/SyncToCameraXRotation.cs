using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncToCameraXRotation : MonoBehaviour
{
    [SerializeField] Transform camTransform;
    PhotonView PV;

    private void Awake()
    {
        PV = transform.parent.parent.GetComponent<PhotonView>();

        if (PV.IsMine)
        {
            //camTransform = transform.parent.transform.Find("Camera Offset").Find("Main Camera").transform;
            //camTransform = transform.parent.transform.Find("Tracking").transform;
            //camTransform = transform.parent.transform.Find("Camera Offset/Tracking");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PV.IsMine)
        {
            // Sync the Y rotation of the model with the camera's Y rotation
            Vector3 currentLocalRotation = transform.localRotation.eulerAngles;
            currentLocalRotation.x = camTransform.localRotation.eulerAngles.x;
            //currentRotation.z = camTransform.rotation.eulerAngles.z;
            transform.localRotation = Quaternion.Euler(currentLocalRotation);
        }

    }
}
