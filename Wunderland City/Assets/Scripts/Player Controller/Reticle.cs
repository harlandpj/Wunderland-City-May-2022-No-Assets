using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

public class Reticle : MonoBehaviour
{
    public CinemachineVirtualCamera cameraFacing;
 
    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = cameraFacing.transform.position + cameraFacing.transform.rotation * Vector3.forward * 10.0f;  // position it in front of player follow camera
        transform.LookAt(cameraFacing.transform.position); // rotate and face the camera
        transform.Rotate(0f, 180f, 0f);
    }
}
