using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameButton;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected == false)
        {
            Debug.Log("Connecting to Master");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

                   

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    
    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("title");
        Debug.Log("Joined Lobby");
        //PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
    }

    public void CreateRoom()
    {
        if(string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        /*
        //!!! This needs to be tested with at least 5 players
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;  // Set the room to allow a maximum of 4 players

        options.IsVisible = true;
        options.IsOpen = true;
        */
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        MenuManager.Instance.OpenMenu("error");
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} left the room.");
        // Optional: update UI or perform cleanup as needed
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("title");
    }


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Updating Room List. Total rooms in list: " + roomList.Count);

        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }

        
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
            {
                continue;
            }

            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);

            /*
            //!!! This needs to be tested with at least 5 players
            var roomListItem = Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>();
            roomListItem.SetUp(roomList[i]);

            // Disable the button if the room is full
            if (roomList[i].PlayerCount >= roomList[i].MaxPlayers)
            {
                roomListItem.GetComponent<Button>().interactable = false;
            }*/
        }

        /*
        // Loop through all rooms provided in the update
        foreach (RoomInfo roomInfo in roomList)
        {
            Debug.Log($"Room: {roomInfo.Name}, Players: {roomInfo.PlayerCount}/{roomInfo.MaxPlayers}, Removed: {roomInfo.RemovedFromList}");

            if (roomInfo.RemovedFromList)
            {
                Debug.Log($"Room {roomInfo.Name} has been removed, skipping.");
                continue;
            }

            // Instantiate new room list item and log
            var roomListItem = Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>();
            roomListItem.SetUp(roomInfo);
            Debug.Log($"Room {roomInfo.Name} instantiated in list.");

            // Disable the button if the room is full
            if (roomInfo.PlayerCount >= roomInfo.MaxPlayers)
            {
                roomListItem.GetComponent<Button>().interactable = false;
                Debug.Log($"Room {roomInfo.Name} is full, button disabled.");
            }
        }*/
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
}
