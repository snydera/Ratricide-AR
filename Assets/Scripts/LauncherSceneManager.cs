using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LauncherSceneManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject roomManagerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        //PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "RoomManager"), Vector3.zero, Quaternion.identity);
        Instantiate(roomManagerPrefab, Vector3.zero, Quaternion.identity);
    }

}
