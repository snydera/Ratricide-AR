using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    Spawnpoint[] spawnpoints;
    private Dictionary<int, Transform> playerSpawnpoints = new Dictionary<int, Transform>(); // Store player spawnpoints

    private void Awake()
    {
        Instance = this;
        spawnpoints = GetComponentsInChildren<Spawnpoint>();
    }

    public Transform GetSpawnpoint(int playerID)
    {
        //return spawnpoints[Random.Range(0, spawnpoints.Length)].transform;

        // If the player already has an assigned spawnpoint, return it
        if (playerSpawnpoints.ContainsKey(playerID))
        {
            return playerSpawnpoints[playerID];
        }

        // Assign a unique spawnpoint to the player
        foreach (Spawnpoint sp in spawnpoints)
        {
            if (!playerSpawnpoints.ContainsValue(sp.transform))
            {
                playerSpawnpoints[playerID] = sp.transform;
                return sp.transform;
            }
        }

        // If all spawnpoints are taken, return a random one as fallback
        return spawnpoints[Random.Range(0, spawnpoints.Length)].transform;
    }
}
