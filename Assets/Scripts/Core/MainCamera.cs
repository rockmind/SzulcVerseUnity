using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public int recursionLimit = 5;
    Portal[] portals;
    Camera playerCam;

    void Awake()
    {
        playerCam = Camera.main;
        portals = FindObjectsOfType<Portal>();
    }

    void OnPreCull()
    {
        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].PrePortalRender();
        }

        for (int i = 0; i < portals.Length; i++)
        {
            List<Portal> otherPortals = portals.ToList().FindAll(x => x!=portals[i] && x!=portals[i].linkedPortal);
            portals[i].Render(
                recursionLimit,
                recursionLimit,
                playerCam.transform.localToWorldMatrix,
                otherPortals,
                playerCam
            );
            // portals[i].Render (recursionLimit);
        }

        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].PostPortalRender();
        }
    }
}