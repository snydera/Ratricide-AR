using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        //PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "PlayerController_Tutorial"), Vector3.zero, Quaternion.identity);
        PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "AR PlayerController"), new Vector3(0f, 1f, 0f), Quaternion.identity);
    }
}
