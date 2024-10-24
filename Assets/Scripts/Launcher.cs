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

    private Dictionary<string, GameObject> existingRooms = new();

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
        
        //!!! This needs to be tested with at least 5 players
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;  // Set the room to allow a maximum of 4 players

        options.IsVisible = true;
        options.IsOpen = true;
        
        PhotonNetwork.CreateRoom(roomNameInputField.text, options, null);
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

        foreach (RoomInfo room in roomList)
        {

            // This "room" changed in some way
            if (room.RemovedFromList)
            {
                // It was removed. Remove it from the menu.
                if (existingRooms.ContainsKey(room.Name))
                {
                    Destroy(existingRooms[room.Name]);
                    existingRooms.Remove(room.Name);
                }
            }
            else
            {
                // It was added, or updated.
                if (!existingRooms.TryGetValue(room.Name, out GameObject obj))
                {
                    // We don't have one yet. Create a new gameObject
                    obj = Instantiate(roomListItemPrefab, roomListContent);
                    obj.GetComponent<Button>().onClick.AddListener(delegate { PhotonNetwork.JoinRoom(room.Name); });

                    existingRooms[room.Name] = obj;
                }
                // Update the text with the new values
                obj.transform.Find("Name of Room").GetComponent<TextMeshProUGUI>().text = room.Name;
                obj.transform.Find("players in room").GetComponent<TextMeshProUGUI>().text = room.PlayerCount + "/" + room.MaxPlayers;

                // Disable the button if the room is full
                // !!! Need to test with at least 5 players
                if (room.PlayerCount >= room.MaxPlayers)
                {
                    obj.GetComponent<Button>().interactable = false;
                    Debug.Log($"Room {room.Name} is full, button disabled.");
                }
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
    }
}
