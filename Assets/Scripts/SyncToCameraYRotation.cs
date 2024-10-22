using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SyncToCameraYRotation : MonoBehaviour
{
    [SerializeField] Transform camTransform;
    PhotonView PV;

    private void Awake()
    {
        PV = transform.parent.GetComponent<PhotonView>();

        if( PV.IsMine)
        {
            //camTransform = transform.parent.transform.Find("Camera Offset").Find("Main Camera").transform;
            camTransform = transform.parent.transform.Find("Camera Offset").Find("Tracking").transform;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if( PV.IsMine )
        {
            // Sync the Y rotation of the model with the camera's Y rotation
            Vector3 currentRotation = transform.rotation.eulerAngles;
            currentRotation.y = camTransform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(currentRotation);
        }
        
    }
}
