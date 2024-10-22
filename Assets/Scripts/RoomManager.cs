using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    private float heartbeatInterval = 5f; // Interval in seconds between heartbeats
    private float lastHeartbeatTime = 0f; // Time of last heartbeat

    private Dictionary<int, float> playerLastHeartbeat = new Dictionary<int, float>(); // Store last heartbeat time for each player

    private void Awake()
    {
        if (Instance) //checks if another RoomManager exists
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void Update()
    {
        // If master client, check heartbeats
        if (PhotonNetwork.IsMasterClient)
        {
            CheckPlayerHeartbeats();
        }

        // Send your heartbeat
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && Time.time - lastHeartbeatTime > heartbeatInterval)
        {
            lastHeartbeatTime = Time.time;
            photonView.RPC("ReceiveHeartbeat", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

    // Master client receives heartbeat from a player
    [PunRPC]
    void ReceiveHeartbeat(int playerID)
    {
        playerLastHeartbeat[playerID] = Time.time;
    }

    // Check player heartbeats and disconnect inactive players
    void CheckPlayerHeartbeats()
    {
        float timeoutDuration = 10f; // Maximum time allowed without heartbeat
        List<int> playersToDisconnect = new List<int>();

        foreach (var player in playerLastHeartbeat)
        {
            if (Time.time - player.Value > timeoutDuration)
            {
                // Player hasn't sent a heartbeat in a while
                playersToDisconnect.Add(player.Key);
            }
        }

        // Disconnect players that have timed out
        foreach (int playerID in playersToDisconnect)
        {
            Photon.Realtime.Player player = PhotonNetwork.CurrentRoom.GetPlayer(playerID);
            if (player != null)
            {
                PhotonNetwork.CloseConnection(player); // Disconnect the unresponsive player
            }
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        /*
        if (scene.buildIndex == 1) // We're in the game scene
        {
            Debug.Log("Scene 1 Loaded");
            PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }*/

        if (scene.buildIndex == 1) // We're in the game scene
        {
            Debug.Log("Scene 1 Loaded");
            PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);

            // If the master client, generate and share the seed
            if (PhotonNetwork.IsMasterClient)
            {
                int seed = Random.Range(0, 1000000); // Generate a random seed
                photonView.RPC("RPC_SetMazeSeed", RpcTarget.All, seed); // Send the seed to all clients
            }
        }
    }

    // RPC for setting the maze seed across all clients
    [PunRPC]
    public void RPC_SetMazeSeed(int seed)
    {
        // Find the MazeGenerator in the scene and pass the seed
        MazeGenerator mazeGenerator = FindObjectOfType<MazeGenerator>();
        if (mazeGenerator != null)
        {
            mazeGenerator.StartMazeGeneration(seed); // Call the method with the seed
        }
        else
        {
            Debug.LogError("MazeGenerator not found in the scene.");
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} has disconnected.");

        // Let the PlayerManager handle its own cleanup
        PlayerManager leavingPlayerManager = PlayerManager.Find(otherPlayer);
        if (leavingPlayerManager != null)
        {
            Debug.Log($"Cleaned up {otherPlayer.NickName}'s player manager.");
        }
    }

    // Optionally handle when the local player (yourself) leaves the room
    public void LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0); // Or whatever your lobby scene is
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left the room.");
        SceneManager.LoadScene(0); // Back to lobby
    }
}
