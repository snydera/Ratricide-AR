using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncToCameraXRotation : MonoBehaviour
{
    [SerializeField] Transform camTransform;
    [SerializeField] Transform aimTarget;
    [SerializeField] PhotonView PV;

    private void Awake()
    {
        //PV = transform.parent.parent.GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*
        if (PV.IsMine)
        {
            
            // Sync the Y rotation of the model with the camera's Y rotation
            Vector3 currentLocalRotation = transform.localRotation.eulerAngles;
            currentLocalRotation.x = camTransform.localRotation.eulerAngles.x;
            //currentRotation.z = camTransform.rotation.eulerAngles.z;
            transform.localRotation = Quaternion.Euler(currentLocalRotation);
            


        }*/
        /*
        if (camTransform == null || aimTarget == null) return;

        // Sync Y rotation of Aim Target Origin with camTransform
        Vector3 originRotation = transform.localRotation.eulerAngles;
        originRotation.y = camTransform.localRotation.eulerAngles.y;
        transform.localRotation = Quaternion.Euler(originRotation);

        // Sync X rotation of aimTarget with camTransform
        Vector3 aimTargetRotation = aimTarget.localRotation.eulerAngles;
        aimTargetRotation.x = camTransform.localRotation.eulerAngles.x;
        aimTarget.localRotation = Quaternion.Euler(aimTargetRotation);*/

        transform.rotation = camTransform.rotation;

    }
}
