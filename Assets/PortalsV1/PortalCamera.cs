using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    public Transform worldB;
    public Transform playerCamera;
    public Transform portalA;
    public Transform portalB;
    
    void Update()
    {
        Transform cameraTransform = transform;
        Quaternion worldBRotation = worldB.rotation;
        Vector3 playerOffsetFromPortal = worldBRotation * (playerCamera.position - portalA.position);

        cameraTransform.rotation = worldBRotation * playerCamera.rotation;
        cameraTransform.position = portalB.position + playerOffsetFromPortal;

        // float angularDifferenceBetweenPortalRotations = Quaternion.Angle(portalB.rotation, portalA.rotation);
  
        // Quaternion portalRotationDifference = Quaternion.AngleAxis(angularDifferenceBetweenPortalRotations, Vector3.up);
        // Vector3 newCameraDirection = portalRotationDifference * playerCamera.forward;
        // transform.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);
    }
}
