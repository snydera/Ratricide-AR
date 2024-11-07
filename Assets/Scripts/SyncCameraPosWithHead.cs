using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncCameraPosWithHead : MonoBehaviour
{
    [SerializeField] Transform headBone;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = headBone.position;
    }
}
