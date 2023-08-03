using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public int recursionLimit = 5;
    List<Portal> portals;
    Camera playerCam;

    void Awake()
    {
        playerCam = Camera.main;
        portals = FindObjectsOfType<Portal>().ToList();
    }

    void OnPreCull()
    {
        foreach (Portal portal in portals)
        {
            portal.PrePortalRender();
        }

        foreach (Portal portal in portals)
        {
            portal.Render(
                recursionLimit,
                playerCam.transform.localToWorldMatrix,
                portals,
                playerCam
            );
            // portals[i].Render (recursionLimit);
        }

        foreach (Portal portal in portals)
        {
            portal.PostPortalRender();
        }
    }
}