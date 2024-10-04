using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ARPlayer : MonoBehaviourPunCallbacks
{
    bool lookLeft;
    bool lookRight;
    bool moveForward;
    bool moveBackward;
    bool moveLeft;
    bool moveRight;

    bool grounded;
    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;

    public float moveSpeed = 5f;
    public float turnSpeed = 5f;

    public Transform gyroCamera;

    TrackedPoseDriver trackedPoseDriver;
    Rigidbody rb;
    PhotonView PV;

    [SerializeField] Item[] items;

    bool item0Equipped = false;
    
    [SerializeField]
    int itemIndex = 1;
    int previousItemIndex = -1;

    private void Awake()
    {
        trackedPoseDriver = transform.Find("Camera Offset").Find("Main Camera").GetComponent<TrackedPoseDriver>();
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
    }
    private void Start()
    {
        if (PV.IsMine)
        {
            // Randomize the spawn position to avoid overlapping
            float randomOffset = Random.Range(-2f, 2f); // Adjust the range as needed
            transform.position += new Vector3(randomOffset, 0, randomOffset);

            EquipItem(0);
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine)
            return;

        Movement();
    }

    private void FixedUpdate()
    {
        if (!PV.IsMine)
            return;

        // https://www.youtube.com/watch?v=AZRdwnBJcfg&list=PLhsVv9Uw1WzjI8fEBjBQpTyXNZ6Yp1ZLw&index=7&ab_channel=RugbugRedfern at 13:50
        // will need to swtich AR Player Movement to fixed Update and Time.fixedDeltaTime
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    #region Pointer Event bools
    public void PointerDownLookLeft()
    {
        lookLeft = true;
    }

    public void PointerUpLookLeft()
    {
        lookLeft = false;
    }

    public void PointerDownLookRight()
    {
        lookRight = true;
    }

    public void PointerUpLookRight()
    {
        lookRight = false;
    }

    public void PointerDownMoveForward()
    {
        moveForward = true;

    }

    public void PointerUpMoveForward()
    {
        moveForward = false;
    }

    public void PointerDownMoveBackward()
    {
        moveBackward = true;
    }

    public void PointerUpMoveBackward()
    {
        moveBackward = false;
    }

    public void PointerDownMoveLeft()
    {
        moveLeft = true;
    }

    public void PointerUpMoveLeft()
    {
        moveLeft = false;
    }

    public void PointerDownMoveRight()
    {
        moveRight = true;
    }

    public void PointerUpMoveRigh()
    {
        moveRight = false;
    }

    #endregion

    #region Movement and Rotation

    public void Movement()
    {
        /*
        float rotationY = gyroCamera.rotation.eulerAngles.y;

        if (lookLeft)
        {
            //transform.Rotate(new Vector3(0, -turnSpeed, 0));
            //RotateCamera(-turnSpeed * Time.deltaTime);
            RotateParent(-turnSpeed * Time.fixedDeltaTime);
        }
        else if (lookRight)
        {
            //transform.Rotate(new Vector3(0, turnSpeed, 0));
            //RotateCamera(turnSpeed * Time.deltaTime);
            RotateParent(turnSpeed * Time.fixedDeltaTime);
        }


        if (moveForward)
        {
            //transform.position += transform.forward * Time.deltaTime * moveSpeed;
            MovePlayerPosition(rotationY);
        }
        else if (moveBackward)
        {
            //transform.position -= transform.forward * Time.deltaTime * moveSpeed;
            MovePlayerPosition(rotationY + 180f);
        }
        
        if (moveLeft)
        {
            MovePlayerPosition(rotationY - 90f);
        }
        else if (moveRight)
        {
            MovePlayerPosition(rotationY + 90f);
        }*/

        Vector3 moveDir = Vector3.zero;

        if (moveForward)
        {
            moveDir += Vector3.forward;
        }
        if (moveBackward)
        {
            moveDir += Vector3.back;
        }
        if (moveLeft)
        {
            moveDir += Vector3.left;
        }
        if (moveRight)
        {
            moveDir += Vector3.right;
        }

        // Calculate movement direction and smooth it out over time
        moveDir = moveDir.normalized;
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * moveSpeed, ref smoothMoveVelocity, 0.1f);  // 0.1f for smoothTime
    }

    private void MovePlayerPosition(float rotationY)
    {
        // Calculate the movement direction based on the rotation
        Vector3 movementDirection = new Vector3(Mathf.Sin(rotationY * Mathf.Deg2Rad), 0f, Mathf.Cos(rotationY * Mathf.Deg2Rad));

        // Move the player in the calculated direction
        transform.Translate(movementDirection * moveSpeed * Time.fixedDeltaTime);
    }

    private void RotateCamera(float rotationAmount)
    {
        // Rotate the gyro camera around the Y-axis
        gyroCamera.Rotate(0f, rotationAmount, 0f);
    }

    private void RotateParent(float rotationAmount)
    {
        // Rotate the parent object around the Y-axis
        this.transform.Rotate(0f, rotationAmount, 0f);
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

        /*
        if (item0Equipped)
        {
            items[1].itemGameObject.SetActive(true);
            items[0].itemGameObject.SetActive(false);
            item0Equipped = false;
            itemIndex = 1;
        }
        else
        {
            items[0].itemGameObject.SetActive(true);
            items[1].itemGameObject.SetActive(false);
            item0Equipped = true;
            itemIndex = 0;
        }*/

        if (PV.IsMine) //!!!
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
}
