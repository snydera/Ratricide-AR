using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour, IDamageable
{
    [SerializeField]
    public GameObject leftWall;

    [SerializeField]
    public GameObject rightWall;

    [SerializeField]
    public GameObject frontWall;

    [SerializeField]
    public GameObject backWall;

    [SerializeField]
    private GameObject unvisitedBlock;

    public bool IsVisited { get; private set; }

    public void Visit()
    {
        IsVisited = true;
        unvisitedBlock.SetActive(false);
    }

    public void ClearLeftWall()
    {
        leftWall.SetActive(false);
    }

    public void ClearRightWall()
    {
        rightWall.SetActive(false);
    }

    public void ClearFrontWall()
    {
        frontWall.SetActive(false);
    }

    public void ClearBackWall()
    {
        backWall.SetActive(false);
    }

    // Implementing the IDamageable interface
    public void TakeDamage(float damage)
    {
        // Implement your destruction logic here, like destroying or deactivating the wall
        Debug.Log("Wall took damage: " + damage);
    }
}
