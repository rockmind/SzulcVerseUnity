using System;
using System.Collections.Generic;
using UnityEngine;

public class PortalTraveller : MonoBehaviour {

    public GameObject graphicsObject;
    public Dictionary<Tuple<Portal, Portal>, GameObject> graphicsClonesMap = new();
    public Vector3 previousOffsetFromPortal { get; set; }

    public Dictionary<Tuple<Portal, Portal>, Material[]> originalMaterialsMap = new();
    public Dictionary<Tuple<Portal, Portal>, Material[]> cloneMaterialsMap = new();

    public virtual void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
        transform.position = pos;
        transform.rotation = rot;
    }

    // Called when first touches portal
    public virtual void EnterPortalThreshold (Tuple<Portal, Portal> portalPair) {
        if (graphicsClonesMap.ContainsKey(portalPair)) {
            graphicsClonesMap[portalPair].SetActive (true);
        }
        else {
            GameObject graphicsClone = Instantiate (graphicsObject);
            graphicsClone.transform.parent = graphicsObject.transform.parent;
            graphicsClone.transform.localScale = graphicsObject.transform.localScale;
            graphicsClone.transform.rotation = graphicsObject.transform.rotation;
            
            originalMaterialsMap[portalPair] = GetMaterials (graphicsObject);
            cloneMaterialsMap[portalPair] = GetMaterials (graphicsClone);
            graphicsClonesMap[portalPair] = graphicsClone;
        }
    }

    // Called once no longer touching portal (excluding when teleporting)
    public virtual void ExitPortalThreshold (Tuple<Portal, Portal> portalPair) {
        graphicsClonesMap[portalPair].SetActive (false);
        // Disable slicing
        for (int i = 0; i < originalMaterialsMap[portalPair].Length; i++) {
            originalMaterialsMap[portalPair][i].SetVector ("sliceNormal", Vector3.zero);
        }
    }

    public void SetSliceOffsetDst (Tuple<Portal, Portal> portalPair, float dst, bool clone) {
        for (int i = 0; i < originalMaterialsMap[portalPair].Length; i++) {
            if (clone) {
                cloneMaterialsMap[portalPair][i].SetFloat ("sliceOffsetDst", dst);
            } else {
                originalMaterialsMap[portalPair][i].SetFloat ("sliceOffsetDst", dst);
            }

        }
    }

    Material[] GetMaterials (GameObject g) {
        var renderers = g.GetComponentsInChildren<MeshRenderer> ();
        var matList = new List<Material> ();
        foreach (var renderer in renderers) {
            foreach (var mat in renderer.materials) {
                matList.Add (mat);
            }
        }
        return matList.ToArray ();
    }
}