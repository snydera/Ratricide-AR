using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Realtime;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    PhotonView PV;

    GameObject controller;

    int kills;
    int deaths;

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
        //Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        //controller = PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "PlayerController_Tutorial"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });

        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint(PV.Owner.ActorNumber); // Use player ID
        controller = PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "AR PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
    }

    public void Die()
    {
        PhotonNetwork.Destroy(controller);      
        CreateController();

        deaths++;

        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public void GetKill()
    {
        PV.RPC(nameof(RPC_GetKill), PV.Owner);
    }

    [PunRPC]
    void RPC_GetKill()
    {
        kills++;

        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PV.Owner == otherPlayer && controller != null)
        {
            PhotonNetwork.Destroy(controller); // Destroy the correct controller for the leaving player
        }
    }

    // Optionally handle when the local player (yourself) leaves the room
    public void LeaveGame()
    {
        /*
        if (PV.IsMine)
        {
            PhotonNetwork.LeaveRoom();
            //SceneManager.LoadScene(0); // Or whatever your lobby scene is
        }*/
        
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                MigrateMaster();
                
            }
            else
            {
                PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
                FindObjectOfType<ARSession>().Reset();

            }

            PhotonNetwork.LeaveRoom();
        }
        // Reset AR session for remaining players
        if (PhotonNetwork.IsMasterClient)
        {
             // Assuming ARSession is part of your XR setup
                               // Re-enable TrackedPoseDriver if necessary
        }
    }

    private void MigrateMaster()
    {
        var dict = PhotonNetwork.CurrentRoom.Players;
        if (PhotonNetwork.SetMasterClient(dict[dict.Count - 1]))
            PhotonNetwork.LeaveRoom();
    }
}
