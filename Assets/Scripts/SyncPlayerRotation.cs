using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPlayerRotation : MonoBehaviourPun
{
    //public Transform cameraTransform;
    public Transform modelTransform;
    public Transform spineTransform;
    public Transform upperChestTransform;
    public Transform headTransform;

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            // Send the local player's camera rotation to the network
            //photonView.RPC("UpdateRotation", RpcTarget.Others, cameraTransform.rotation);
            photonView.RPC("UpdateRotation", RpcTarget.Others, modelTransform.rotation);
            photonView.RPC("UpdateConstraintTransforms", RpcTarget.Others, spineTransform.rotation);
            photonView.RPC("UpdateConstraintTransforms", RpcTarget.Others, upperChestTransform.rotation);
            photonView.RPC("UpdateConstraintTransforms", RpcTarget.Others, headTransform.rotation);
        }
    }

    [PunRPC]
    void UpdateRotation(Quaternion newRotation)
    {
        // Apply the received rotation to the camera
        //cameraTransform.rotation = newRotation;
        modelTransform.rotation = newRotation;
    }

    [PunRPC]
    void UpdateConstraintTransforms(Transform constrainedTransform, Quaternion newRotation)
    {
        // Apply the received rotation to the camera
        //cameraTransform.rotation = newRotation;
        constrainedTransform.rotation = newRotation;
    }

    //Received RPC "UpdateRotation" for viewID 4002 but this PhotonView does not exist! Was remote PV. Owner called.
    //By: #04 'IPhone' Maybe GO was destroyed but RPC not cleaned up.
}
