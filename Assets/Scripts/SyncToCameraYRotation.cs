using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

public class SyncToCameraYRotation : MonoBehaviour
{
    [SerializeField] Transform camTransform;
    [SerializeField] Transform aimTarget;
    PhotonView PV;

    private void Awake()
    {
        PV = transform.parent.GetComponent<PhotonView>();

        if( PV.IsMine)
        {
            //camTransform = transform.parent.transform.Find("Camera Offset").Find("Main Camera").transform;
            camTransform = transform.parent.transform.Find("Camera Offset").Find("Tracking").transform;

            foreach (MultiAimConstraint component in GetComponentsInChildren<MultiAimConstraint>())
            {
                var data = component.data.sourceObjects;
                data.SetTransform(0, camTransform.transform.Find("Aim Target Origin").Find("Aim Target").transform);
                component.data.sourceObjects = data;
            }
            RigBuilder rigs = GetComponentInChildren<RigBuilder>();
            rigs.Build();
        }
        else
        {
            // Disable Aim Target for remote players
            aimTarget.gameObject.SetActive(false);
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

            // Sync Aim Target position and rotation over network
            PV.RPC("SyncAimTargetTransform", RpcTarget.Others, aimTarget.position, aimTarget.rotation);
        }
        
    }

    // RPC to update Aim Target position and rotation for remote players
    [PunRPC]
    private void SyncAimTargetTransform(Vector3 position, Quaternion rotation)
    {
        aimTarget.position = position;
        aimTarget.rotation = rotation;
    }
}
