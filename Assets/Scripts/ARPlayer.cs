using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
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
    
    //[SerializeField] Renderer headRenderer;
    [SerializeField] GameObject headMesh;

    [SerializeField] Image healthbarImage;

    [SerializeField] Item[] items;

    public int itemIndex = 1;
    int previousItemIndex = -1;

    const float maxHealth = 100f;
    public float currentHealth = maxHealth;

    PlayerManager playerManager;

    [SerializeField] Animator anim;

    private void Awake()
    {
        //trackedPoseDriver = gyroCamera.GetComponent<TrackedPoseDriver>();
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        canvas = transform.Find("Canvas").GetComponent<Canvas>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }
    private void Start()
    {
        if (PV.IsMine)
        {
            gyroCamera.GetComponent<Camera>().enabled = true;

            //trackedPoseDriver.enabled = true;
            //headRenderer.enabled = false;
            //headMesh.layer = 7;
            headMesh.GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            
            
            /*
            // Randomize the spawn position to avoid overlapping
            float randomOffset = Random.Range(-2f, 2f); // Adjust the range as needed
            transform.position += new Vector3(randomOffset, 0, randomOffset);
            */

            EquipItem(0);
        }
        else
        {
            Destroy(canvas.gameObject);

            Destroy(gyroCamera.gameObject);
            /*
            gyroCamera.GetComponent<Camera>().enabled = false;
            Destroy(gyroCamera.GetComponent<Camera>());
            Destroy(gyroCamera.GetComponent<AudioListener>());
            Destroy(gyroCamera.GetComponent<ARCameraManager>());
            Destroy(gyroCamera.GetComponent<ARCameraBackground>());
            Destroy(gyroCamera.GetComponent<PostProcessLayer>());
            */
            
            /*
            if (trackedPoseDriver != null)
            {
                trackedPoseDriver.enabled = false;
            }*/

            Destroy(rb);
        }

        OffsetCameraRotation();
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

        if (transform.position.y < -10f)
        {
            Die();
        }

        /*
        //!!! remove before build
        if (currentHealth <= 0)
        {
            anim.SetBool("isDead", true);
            StartCoroutine(DeathRoutine());
        }*/
    }

    void OffsetCameraRotation()
    {
        transform.Find("Camera Offset").transform.localRotation = transform.rotation;
        transform.rotation = Quaternion.identity;
    }

    #region Pointer Event bools
    public void PointerDownMoveForward()
    {
        if (PV.IsMine)
        {
            moveForward = true;
            anim.SetBool("isMovingForward", true);
        }
    }

    public void PointerUpMoveForward()
    {
        if (PV.IsMine)
        {
            moveForward = false;
            anim.SetBool("isMovingForward", false);
        }
    }

    public void PointerDownMoveBackward()
    {
        if (PV.IsMine)
        {
            moveBackward = true;
            anim.SetBool("isMovingBackward", true);
        }
    }

    public void PointerUpMoveBackward()
    {
        if (PV.IsMine)
        {
            moveBackward = false;
            anim.SetBool("isMovingBackward", false);
        }
    }

    public void PointerDownMoveLeft()
    {
        if (PV.IsMine)
        {
            moveLeft = true;
            anim.SetBool("isStrafingLeft", true);
        }
    }

    public void PointerUpMoveLeft()
    {
        if (PV.IsMine)
        {
            moveLeft = false;
            anim.SetBool("isStrafingLeft", false);
        }
    }

    public void PointerDownMoveRight()
    {
        if (PV.IsMine)
        {
            moveRight = true;
            anim.SetBool("isStrafingRight", true);
        }
    }

    public void PointerUpMoveRight()
    {
        if (PV.IsMine)
        {
            moveRight = false;
            anim.SetBool("isStrafingRight", false);
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


    public void EquipItem(int _index)
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
        if (PV.IsMine)
        {
            anim.SetInteger("ItemIndex", itemIndex);
        }
        
    }

    public void FireButtonPressed()
    {
        if (PV.IsMine)
        {
            items[itemIndex].Use();
        }
        
    }

    public void OnLeaveGameButtonPressed()
    {
        if (PV.IsMine)
        {
            playerManager.LeaveGame();  // Call the PlayerManager's LeaveGame method
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("itemIndex") && !PV.IsMine && targetPlayer == PV.Owner)
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
        PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, PhotonMessageInfo info)
    {
        currentHealth -= damage;

        healthbarImage.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            if (!PV.IsMine)
            {
                anim.SetBool("isDead", true);

            }
            StartCoroutine(DeathRoutine());
            PlayerManager.Find(info.Sender).GetKill();
        }
    }

    void Die()
    {
        playerManager.Die();
        OffsetCameraRotation();
        if (PV.IsMine)
        {
            anim.SetBool("isDead", false);
        }
        
    }

    IEnumerator DeathRoutine()
    {
        headMesh.GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        canvas.transform.Find("Movement").gameObject.SetActive(false);
        canvas.transform.Find("Actions").gameObject.SetActive(false);
        GetComponent<CapsuleCollider>().enabled = false;

        yield return new WaitForSeconds(4);

        Die();
    }

    public PlayerState GetPlayerState()
    {
        return new PlayerState
        {
            position = transform.position,
            rotation = transform.rotation,
            health = currentHealth,
            //ammo = items[itemIndex].GetAmmoCount() // Assuming GetAmmoCount exists in the item
        };
    }

    public void SetPlayerState(PlayerState state)
    {
        transform.position = state.position;
        transform.rotation = state.rotation;
        currentHealth = state.health;
        //items[itemIndex].SetAmmoCount(state.ammo); // Assuming SetAmmoCount exists in the item
        healthbarImage.fillAmount = currentHealth / maxHealth;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        playerManager.Reinstantiate(transform.position, transform.rotation);
    }
}

// Define a PlayerState class to hold player state information
[System.Serializable]
public class PlayerState
{
    public Vector3 position;
    public Quaternion rotation;
    public float health;
    public int ammo;  // Assuming the player has an ammo count
}
