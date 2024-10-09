using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ARPlayer : MonoBehaviourPunCallbacks, IDamageable
{
    bool moveForward;
    bool moveBackward;
    bool moveLeft;
    bool moveRight;
    bool isFiringWeapon;

    bool grounded;

    public float moveSpeed = 5f;
    public float turnSpeed = 5f;

    public Transform gyroCamera;

    TrackedPoseDriver trackedPoseDriver;
    Rigidbody rb;
    PhotonView PV;
    Canvas canvas;

    [SerializeField] Item[] items;

    int itemIndex = 1;
    int previousItemIndex = -1;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;

    PlayerManager playerManager;

    private void Awake()
    {
        trackedPoseDriver = gyroCamera.GetComponent<TrackedPoseDriver>();
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        canvas = transform.Find("Canvas").GetComponent<Canvas>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }
    private void Start()
    {
        if (PV.IsMine)
        {
            canvas.gameObject.SetActive(true);
            gyroCamera.GetComponent<Camera>().enabled = true;
            trackedPoseDriver.enabled = true;
            
            // Randomize the spawn position to avoid overlapping
            float randomOffset = Random.Range(-2f, 2f); // Adjust the range as needed
            transform.position += new Vector3(randomOffset, 0, randomOffset);

            EquipItem(0);
        }
        else
        {
            canvas.gameObject.SetActive(false);
            gyroCamera.GetComponent<Camera>().enabled = false;
            
            if (trackedPoseDriver != null)
            {
                trackedPoseDriver.enabled = false;
            }

            Destroy(rb);
        }
    }


    private void FixedUpdate()
    {
        if (!PV.IsMine)
            return;

        Movement();

        if (isFiringWeapon)
        {
            items[itemIndex].Use();
        }

        if (transform.position.y < 10f)
        {
            Die();
        }
    }

    #region Pointer Event bools
    public void PointerDownMoveForward()
    {
        if (PV.IsMine)
        {
            moveForward = true;
        }
    }

    public void PointerUpMoveForward()
    {
        if (PV.IsMine)
        {
            moveForward = false;
        }
    }

    public void PointerDownMoveBackward()
    {
        if (PV.IsMine)
        {
            moveBackward = true;
        }
    }

    public void PointerUpMoveBackward()
    {
        if (PV.IsMine)
        {
            moveBackward = false;
        }
    }

    public void PointerDownMoveLeft()
    {
        if (PV.IsMine)
        {
            moveLeft = true;
        }
    }

    public void PointerUpMoveLeft()
    {
        if (PV.IsMine)
        {
            moveLeft = false;
        }
    }

    public void PointerDownMoveRight()
    {
        if (PV.IsMine)
        {
            moveRight = true;
        }
    }

    public void PointerUpMoveRigh()
    {
        if (PV.IsMine)
        {
            moveRight = false;
        }
    }

    public void PointerDownFireWeapon()
    {
        if (PV.IsMine)
        {
            isFiringWeapon = true;
        }
    }

    public void PointerUpFireWeapon()
    {
        if (PV.IsMine)
        {
            isFiringWeapon = false;
        }
    }

    #endregion

    #region Movement and Rotation

    public void Movement()
    {     
        float rotationY = gyroCamera.rotation.eulerAngles.y;

        if (moveForward)
        {
            MovePlayerPosition(rotationY);
        }
        else if (moveBackward)
        {
            MovePlayerPosition(rotationY + 180f);
        }
        
        if (moveLeft)
        {
            MovePlayerPosition(rotationY - 90f);
        }
        else if (moveRight)
        {
            MovePlayerPosition(rotationY + 90f);
        }
    }

    private void MovePlayerPosition(float rotationY)
    {
        // Calculate the movement direction based on the rotation
        Vector3 movementDirection = new Vector3(Mathf.Sin(rotationY * Mathf.Deg2Rad), 0f, Mathf.Cos(rotationY * Mathf.Deg2Rad));

        // Move the player in the calculated direction
        transform.Translate(movementDirection * moveSpeed * Time.fixedDeltaTime);
    }

    #endregion


    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
            return;

        itemIndex = _index;

        items[itemIndex].itemGameObject.SetActive(true);

        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }

        previousItemIndex = itemIndex;

        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public void WeaponButtonPressed()
    {
        // Toggle the itemIndex between 0 and 1
        itemIndex = (itemIndex == 0) ? 1 : 0;

        // Equip the item based on the updated itemIndex
        EquipItem(itemIndex);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!PV.IsMine && targetPlayer == PV.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    public void TakeDamage(float damage)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        if (!PV.IsMine)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        playerManager.Die();
    }
}
