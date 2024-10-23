using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPlayerRotation : MonoBehaviourPun
{
    //public Transform cameraTransform;
    public Transform modelTransform;

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            // Send the local player's camera rotation to the network
            //photonView.RPC("UpdateRotation", RpcTarget.Others, cameraTransform.rotation);
            photonView.RPC("UpdateRotation", RpcTarget.Others, modelTransform.rotation);
        }
    }

    [PunRPC]
    void UpdateRotation(Quaternion newRotation)
    {
        // Apply the received rotation to the camera
        //cameraTransform.rotation = newRotation;
        modelTransform.rotation = newRotation;
    }

    //Received RPC "UpdateRotation" for viewID 4002 but this PhotonView does not exist! Was remote PV. Owner called.
    //By: #04 'IPhone' Maybe GO was destroyed but RPC not cleaned up.
}
