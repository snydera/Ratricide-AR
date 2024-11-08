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

            // Send each constrained bone's position and rotation
            photonView.RPC("UpdateConstraintTransforms", RpcTarget.Others, "spine", spineTransform.position, spineTransform.rotation);
            photonView.RPC("UpdateConstraintTransforms", RpcTarget.Others, "upperChest", upperChestTransform.position, upperChestTransform.rotation);
            photonView.RPC("UpdateConstraintTransforms", RpcTarget.Others, "head", headTransform.position, headTransform.rotation); ;
        }
    }

    [PunRPC]
    void UpdateRotation(Quaternion newRotation)
    {
        modelTransform.rotation = newRotation;
    }

    [PunRPC]
    void UpdateConstraintTransforms(string boneName, Vector3 newPosition, Quaternion newRotation)
    {
        // Identify the bone by name and update its position and rotation
        Transform targetTransform = null;

        if (boneName == "spine") targetTransform = spineTransform;
        else if (boneName == "upperChest") targetTransform = upperChestTransform;
        else if (boneName == "head") targetTransform = headTransform;

        if (targetTransform != null)
        {
            targetTransform.position = newPosition;
            targetTransform.rotation = newRotation;
        }
    }

    //Received RPC "UpdateRotation" for viewID 4002 but this PhotonView does not exist! Was remote PV. Owner called.
    //By: #04 'IPhone' Maybe GO was destroyed but RPC not cleaned up.
}
