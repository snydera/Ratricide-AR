using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPlayer : MonoBehaviour
{
    bool lookLeft;
    bool lookRight;
    bool moveForward;
    bool moveBackward;
    bool moveLeft;
    bool moveRight;

    public float moveSpeed = 5f;
    public float turnSpeed = 5f;

    public Transform gyroCamera;

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

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
    }

    public void Movement()
    {
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
        }

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
}
