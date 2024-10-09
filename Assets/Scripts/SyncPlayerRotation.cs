using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPlayerRotation : MonoBehaviourPun
{
    public Transform cameraTransform;

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            // Send the local player's camera rotation to the network
            photonView.RPC("UpdateRotation", RpcTarget.Others, cameraTransform.rotation);
        }
    }

    [PunRPC]
    void UpdateRotation(Quaternion newRotation)
    {
        // Apply the received rotation to the camera
        cameraTransform.rotation = newRotation;
    }
}
