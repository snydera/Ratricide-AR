using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPlayer : MonoBehaviour
{
    bool lookLeft;
    bool lookRight;
    bool moveForward;
    bool moveBackward;

    public float moveSpeed = 5f;
    public float turnSpeed = 5f;

    public Transform gyroCamera;


    public void PointerDownLeft()
    {
        lookLeft = true;
    }

    public void PointerUpLeft()
    {
        lookLeft = false;
    }

    public void PointerDownRight()
    {
        lookRight = true;
    }

    public void PointerUpRight()
    {
        lookRight = false;
    }

    public void PointerDownForward()
    {
        moveForward = true;

    }

    public void PointerUpForward()
    {
        moveForward = false;
    }

    public void PointerDownBackward()
    {
        moveBackward = true;
    }

    public void PointerUpBackward()
    {
        moveBackward = false;
    }

    // Update is called once per frame
    void Update()
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
            RotateParent(-turnSpeed * Time.deltaTime);
        }
        else if (lookRight)
        {
            //transform.Rotate(new Vector3(0, turnSpeed, 0));
            //RotateCamera(turnSpeed * Time.deltaTime);
            RotateParent(turnSpeed * Time.deltaTime);
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

    }

    private void MovePlayerPosition(float rotationY)
    {
        // Calculate the movement direction based on the rotation
        Vector3 movementDirection = new Vector3(Mathf.Sin(rotationY * Mathf.Deg2Rad), 0f, Mathf.Cos(rotationY * Mathf.Deg2Rad));

        // Move the player in the calculated direction
        transform.Translate(movementDirection * moveSpeed * Time.deltaTime);
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
