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

    // Handle player disconnection
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} has left the game.");

        // Find and destroy the disconnected player's PlayerManager
        PlayerManager playerManager = PlayerManager.Find(otherPlayer);
        if (playerManager != null)
        {
            Destroy(playerManager.gameObject);
        }

        // Notify remaining players or update game state as needed
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Master client updated after player left.");
            // Potentially assign a new master client or update the game state
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
