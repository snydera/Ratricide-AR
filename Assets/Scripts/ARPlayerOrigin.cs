using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPlayerOrigin : MonoBehaviour
{
    GameObject ARPlayerController;

    private void Awake()
    {
        ARPlayerController = transform.Find("AR PlayerController").gameObject;
    }

    public void ResetARPlayer()
    {
        ARPlayerController.transform.localPosition = Vector3.zero;
        ARPlayerController.transform.localRotation = Quaternion.identity;
    }
}
