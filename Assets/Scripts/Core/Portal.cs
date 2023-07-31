﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Main Settings")] public Portal linkedPortal;
    public MeshRenderer screen;

    [Header("Advanced Settings")] public float nearClipOffset = 0.05f;
    public float nearClipLimit = 0.2f;

    // Private variables
    RenderTexture viewTexture;
    Camera portalCam;
    Camera playerCam;
    Material firstRecursionMat;
    List<PortalTraveller> trackedTravellers;
    MeshFilter screenMeshFilter;

    private Tuple<Portal, Portal> portalPair;

    void Awake()
    {
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera>();
        portalCam.enabled = false;
        trackedTravellers = new List<PortalTraveller>();
        screenMeshFilter = screen.GetComponent<MeshFilter>();
        screen.material.SetInt("displayMask", 1);
        List<Portal> portalPairList = new List<Portal>() { this, linkedPortal };
        portalPairList = portalPairList.OrderBy(x => x.name)
            .ThenBy(x => x.transform.position.x)
            .ThenBy(x => x.transform.position.y)
            .ThenBy(x => x.transform.position.z)
            .ThenBy(x => x.transform.rotation.x)
            .ThenBy(x => x.transform.rotation.y)
            .ThenBy(x => x.transform.rotation.z)
            .ToList();
        portalPair = Tuple.Create<Portal, Portal>(portalPairList[0], portalPairList[1]);
    }

    void LateUpdate()
    {
        HandleTravellers();
    }

    void HandleTravellers()
    {
        for (int i = 0; i < trackedTravellers.Count; i++)
        {
            PortalTraveller traveller = trackedTravellers[i];
            Transform travellerT = traveller.transform;
            var m = linkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix *
                    travellerT.localToWorldMatrix;

            Vector3 offsetFromPortal = travellerT.position - transform.position;
            int portalSide = System.Math.Sign(Vector3.Dot(offsetFromPortal, transform.forward));
            int portalSideOld = System.Math.Sign(Vector3.Dot(traveller.previousOffsetFromPortal, transform.forward));
            // Teleport the traveller if it has crossed from one side of the portal to the other
            if (portalSide != portalSideOld)
            {
                var positionOld = travellerT.position;
                var rotOld = travellerT.rotation;
                traveller.Teleport(transform, linkedPortal.transform, m.GetColumn(3), m.rotation);
                traveller.graphicsClonesMap[portalPair].transform.SetPositionAndRotation(positionOld, rotOld);
                // Can't rely on OnTriggerEnter/Exit to be called next frame since it depends on when FixedUpdate runs
                linkedPortal.OnTravellerEnterPortal(traveller);
                trackedTravellers.RemoveAt(i);
                i--;
            }
            else
            {
                traveller.graphicsClonesMap[portalPair].transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);
                //UpdateSliceParams (traveller);
                traveller.previousOffsetFromPortal = offsetFromPortal;
            }
        }
    }

    // Called before any portal cameras are rendered for the current frame
    public void PrePortalRender()
    {
        foreach (var traveller in trackedTravellers)
        {
            UpdateSliceParams(traveller);
        }
    }

    // public void Render (int recursionLimit) {
    //
    //     // // Skip rendering the view from this portal if player is not looking at the linked portal
    //     // if (!CameraUtility.VisibleFromCamera (linkedPortal.screen, playerCam)) {
    //     //     return;
    //     // }
    //
    //     CreateViewTexture ();
    //
    //     var localToWorldMatrix = playerCam.transform.localToWorldMatrix;
    //     var renderPositions = new Vector3[recursionLimit];
    //     var renderRotations = new Quaternion[recursionLimit];
    //
    //     int startIndex = 0;
    //     portalCam.projectionMatrix = playerCam.projectionMatrix;
    //     for (int i = 0; i < recursionLimit; i++) {
    //         if (i > 0) {
    //             // No need for recursive rendering if linked portal is not visible through this portal
    //             if (!CameraUtility.BoundsOverlap (screenMeshFilter, linkedPortal.screenMeshFilter, portalCam)) {
    //                 break;
    //             }
    //         }
    //         localToWorldMatrix = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * localToWorldMatrix;
    //         int renderOrderIndex = recursionLimit - i - 1;
    //         renderPositions[renderOrderIndex] = localToWorldMatrix.GetColumn (3);
    //         renderRotations[renderOrderIndex] = localToWorldMatrix.rotation;
    //
    //         portalCam.transform.SetPositionAndRotation (renderPositions[renderOrderIndex], renderRotations[renderOrderIndex]);
    //         startIndex = renderOrderIndex;
    //     }
    //
    //     for (int i = startIndex; i < recursionLimit; i++) {
    //
    //         if (i == startIndex) {
    //             linkedPortal.screen.material.SetInt ("displayMask", 0);
    //         }
    //         portalCam.transform.SetPositionAndRotation (renderPositions[i], renderRotations[i]);
    //         // // Hide screen so that camera can see through portal
    //         screen.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
    //         SetNearClipPlane (playerCam);
    //         // HandleClipping ();
    //         portalCam.Render ();
    //         // Unhide objects hidden at start of render
    //         screen.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    //
    //         if (i == startIndex) {
    //             linkedPortal.screen.material.SetInt ("displayMask", 1);
    //         }
    //     }
    // }
    // Manually render the camera attached to this portal
    // Called after PrePortalRender, and before PostPortalRender
    public int Render(
        int recursionLimit,
        int currentRecursion,
        Matrix4x4 camLocalToWorldMatrix,
        List<Portal> otherPortals,
        Camera virtualCamera 
        )
    {
        // Skip rendering the view from this portal if player is not looking at the linked portal
        if (!CameraUtility.VisibleFromCamera(linkedPortal.screen, virtualCamera) || currentRecursion < 0)
        {
            return 0;
        }

        if (recursionLimit == currentRecursion)
        {
            CreateViewTexture();
        }

        var renderPosition = new Vector3();
        var renderRotation = new Quaternion();

        int previousRecursion = 1;
        portalCam.projectionMatrix = virtualCamera.projectionMatrix;

        if (currentRecursion > 0)
        {
            camLocalToWorldMatrix = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix *
                                    camLocalToWorldMatrix;
            // foreach (Portal otherPortal in otherPortals)
            // {
            //     if (
            //         !CameraUtility.BoundsOverlap (
            //             screenMeshFilter,
            //             otherPortal.screenMeshFilter,
            //             portalCam)
            //         )
            //     {
            //         continue;
            //     }
            //     otherPortal.Render(
            //         recursionLimit,
            //         currentRecursion - 1,
            //         portalCam.transform.localToWorldMatrix,
            //         otherPortals,
            //         portalCam
            //     );
            // }
            
            previousRecursion = Render(
                recursionLimit,
                currentRecursion - 1,
                camLocalToWorldMatrix,
                otherPortals,
                portalCam
                );
        }

        renderPosition = camLocalToWorldMatrix.GetColumn(3);
        renderRotation = camLocalToWorldMatrix.rotation;

        portalCam.transform.SetPositionAndRotation(renderPosition, renderRotation);

        if (previousRecursion == 0)
        {
            linkedPortal.screen.material.SetInt("displayMask", 0);
        }

        // Hide screen so that camera can see through portal
        screen.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        SetNearClipPlane(virtualCamera);
        HandleClipping ();
        portalCam.Render();
        // Unhide objects hidden at start of render
        screen.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

        if (previousRecursion == 0)
        {
            linkedPortal.screen.material.SetInt("displayMask", 1);
        }

        return currentRecursion;
    }

    void HandleClipping()
    {
        // There are two main graphical issues when slicing travellers
        // 1. Tiny sliver of mesh drawn on backside of portal
        //    Ideally the oblique clip plane would sort this out, but even with 0 offset, tiny sliver still visible
        // 2. Tiny seam between the sliced mesh, and the rest of the model drawn onto the portal screen
        // This function tries to address these issues by modifying the slice parameters when rendering the view from the portal
        // Would be great if this could be fixed more elegantly, but this is the best I can figure out for now
        const float hideDst = -1000;
        const float showDst = 1000;
        float screenThickness = linkedPortal.ProtectScreenFromClipping(portalCam.transform.position);

        foreach (var traveller in trackedTravellers)
        {
            if (SameSideOfPortal(traveller.transform.position, portalCamPos))
            {
                // Addresses issue 1
                traveller.SetSliceOffsetDst(portalPair, hideDst, false);
            }
            else
            {
                // Addresses issue 2
                traveller.SetSliceOffsetDst(portalPair, showDst, false);
            }

            // Ensure clone is properly sliced, in case it's visible through this portal:
            int cloneSideOfLinkedPortal = -SideOfPortal(traveller.transform.position);
            bool camSameSideAsClone = linkedPortal.SideOfPortal(portalCamPos) == cloneSideOfLinkedPortal;
            if (camSameSideAsClone)
            {
                traveller.SetSliceOffsetDst(portalPair, screenThickness, true);
            }
            else
            {
                traveller.SetSliceOffsetDst(portalPair, -screenThickness, true);
            }
        }

        foreach (var linkedTraveller in linkedPortal.trackedTravellers)
        {
            var travellerPos = linkedTraveller.graphicsObject.transform.position;
            var clonePos = linkedTraveller.graphicsClonesMap[portalPair].transform.position;
            // Handle clone of linked portal coming through this portal:
            bool cloneOnSameSideAsCam = linkedPortal.SideOfPortal(travellerPos) != SideOfPortal(portalCamPos);
            if (cloneOnSameSideAsCam)
            {
                // Addresses issue 1
                linkedTraveller.SetSliceOffsetDst(portalPair, hideDst, true);
            }
            else
            {
                // Addresses issue 2
                linkedTraveller.SetSliceOffsetDst(portalPair, showDst, true);
            }

            // Ensure traveller of linked portal is properly sliced, in case it's visible through this portal:
            bool camSameSideAsTraveller =
                linkedPortal.SameSideOfPortal(linkedTraveller.transform.position, portalCamPos);
            if (camSameSideAsTraveller)
            {
                linkedTraveller.SetSliceOffsetDst(portalPair, screenThickness, false);
            }
            else
            {
                linkedTraveller.SetSliceOffsetDst(portalPair, -screenThickness, false);
            }
        }
    }

    // Called once all portals have been rendered, but before the player camera renders
    public void PostPortalRender()
    {
        foreach (var traveller in trackedTravellers)
        {
            UpdateSliceParams(traveller);
        }

        ProtectScreenFromClipping(playerCam.transform.position);
    }

    void CreateViewTexture()
    {
        if (viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height)
        {
            if (viewTexture != null)
            {
                viewTexture.Release();
            }

            viewTexture = new RenderTexture(Screen.width, Screen.height, 0);
            // Render the view from the portal camera to the view texture
            portalCam.targetTexture = viewTexture;
            // Display the view texture on the screen of the linked portal
            linkedPortal.screen.material.SetTexture("_MainTex", viewTexture);
        }
    }

    // Sets the thickness of the portal screen so as not to clip with camera near plane when player goes through
    float ProtectScreenFromClipping(Vector3 viewPoint)
    {
        float halfHeight = playerCam.nearClipPlane * Mathf.Tan(playerCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfWidth = halfHeight * playerCam.aspect;
        float dstToNearClipPlaneCorner = new Vector3(halfWidth, halfHeight, playerCam.nearClipPlane).magnitude;
        float screenThickness = dstToNearClipPlaneCorner;

        Transform screenT = screen.transform;
        bool camFacingSameDirAsPortal = Vector3.Dot(transform.forward, transform.position - viewPoint) > 0;
        screenT.localScale = new Vector3(screenT.localScale.x, screenT.localScale.y, screenThickness);
        screenT.localPosition = Vector3.forward * screenThickness * ((camFacingSameDirAsPortal) ? 0.5f : -0.5f);
        return screenThickness;
    }

    void UpdateSliceParams(PortalTraveller traveller)
    {
        // Calculate slice normal
        int side = SideOfPortal(traveller.transform.position);
        Vector3 sliceNormal = transform.forward * -side;
        Vector3 cloneSliceNormal = linkedPortal.transform.forward * side;

        // Calculate slice centre
        Vector3 slicePos = transform.position;
        Vector3 cloneSlicePos = linkedPortal.transform.position;

        // Adjust slice offset so that when player standing on other side of portal to the object, the slice doesn't clip through
        float sliceOffsetDst = 0;
        float cloneSliceOffsetDst = 0;
        float screenThickness = screen.transform.localScale.z;

        bool playerSameSideAsTraveller = SameSideOfPortal(playerCam.transform.position, traveller.transform.position);
        if (!playerSameSideAsTraveller)
        {
            sliceOffsetDst = -screenThickness;
        }

        bool playerSameSideAsCloneAppearing = side != linkedPortal.SideOfPortal(playerCam.transform.position);
        if (!playerSameSideAsCloneAppearing)
        {
            cloneSliceOffsetDst = -screenThickness;
        }

        // Apply parameters
        for (int i = 0; i < traveller.originalMaterialsMap[portalPair].Length; i++)
        {
            traveller.originalMaterialsMap[portalPair][i].SetVector("sliceCentre", slicePos);
            traveller.originalMaterialsMap[portalPair][i].SetVector("sliceNormal", sliceNormal);
            traveller.originalMaterialsMap[portalPair][i].SetFloat("sliceOffsetDst", sliceOffsetDst);

            traveller.cloneMaterialsMap[portalPair][i].SetVector("sliceCentre", cloneSlicePos);
            traveller.cloneMaterialsMap[portalPair][i].SetVector("sliceNormal", cloneSliceNormal);
            traveller.cloneMaterialsMap[portalPair][i].SetFloat("sliceOffsetDst", cloneSliceOffsetDst);
        }
    }

    // Use custom projection matrix to align portal camera's near clip plane with the surface of the portal
    // Note that this affects precision of the depth buffer, which can cause issues with effects like screenspace AO
    void SetNearClipPlane(Camera virtualCamera)
    {
        // Learning resource:
        // http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
        Transform clipPlane = transform;
        int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward, transform.position - portalCam.transform.position));

        Vector3 camSpacePos = portalCam.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
        Vector3 camSpaceNormal = portalCam.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
        float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal) + nearClipOffset;

        // Don't use oblique clip plane if very close to portal as it seems this can cause some visual artifacts
        if (Mathf.Abs(camSpaceDst) > nearClipLimit)
        {
            Vector4 clipPlaneCameraSpace =
                new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);

            // Update projection based on new clip plane
            // Calculate matrix with player cam so that player camera settings (fov, etc) are used
            portalCam.projectionMatrix = virtualCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        }
        else
        {
            portalCam.projectionMatrix = virtualCamera.projectionMatrix;
        }
    }

    void OnTravellerEnterPortal(PortalTraveller traveller)
    {
        if (!trackedTravellers.Contains(traveller))
        {
            traveller.EnterPortalThreshold(portalPair);
            traveller.previousOffsetFromPortal = traveller.transform.position - transform.position;
            trackedTravellers.Add(traveller);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var traveller = other.GetComponent<PortalTraveller>();
        if (traveller)
        {
            OnTravellerEnterPortal(traveller);
        }
    }

    void OnTriggerExit(Collider other)
    {
        var traveller = other.GetComponent<PortalTraveller>();
        if (traveller && trackedTravellers.Contains(traveller))
        {
            traveller.ExitPortalThreshold(portalPair);
            trackedTravellers.Remove(traveller);
        }
    }

    /*
     ** Some helper/convenience stuff:
     */

    int SideOfPortal(Vector3 pos)
    {
        return System.Math.Sign(Vector3.Dot(pos - transform.position, transform.forward));
    }

    bool SameSideOfPortal(Vector3 posA, Vector3 posB)
    {
        return SideOfPortal(posA) == SideOfPortal(posB);
    }

    Vector3 portalCamPos
    {
        get { return portalCam.transform.position; }
    }

    void OnValidate()
    {
        if (linkedPortal != null)
        {
            linkedPortal.linkedPortal = this;
        }
    }
}