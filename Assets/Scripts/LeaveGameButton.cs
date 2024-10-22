using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaveGameButton : MonoBehaviourPunCallbacks
{
    Button leaveGameButton;

    PhotonView PV;

    private void Awake()
    {
        leaveGameButton = GetComponent<Button>();
        PV = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (PV.IsMine)
        {
            leaveGameButton.onClick.AddListener(FindObjectOfType<PlayerManager>().LeaveGame);
        }
        
        
            
    }
}
