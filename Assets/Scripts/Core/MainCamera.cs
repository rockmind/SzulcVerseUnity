using UnityEngine;

public class MainCamera : MonoBehaviour {

    public int recursionLimit = 5;
    
    Portal[] portals;

    void Awake ()
    {
        portals = FindObjectsOfType<Portal> ();
        foreach (Portal portal in portals)
        {
            portal.CreatePositionTables(recursionLimit);
        }
    }

    void OnPreCull () {

        for (int i = 0; i < portals.Length; i++) {
            portals[i].PrePortalRender (recursionLimit);
        }

        for (int recursionStep = 0; recursionStep < recursionLimit; recursionStep++)
        {
            for (int i = 0; i < portals.Length; i++)
            {
                for (int j = 0; j < portals.Length; j++)
                {
                    portals[j].Render(recursionStep);
                }
                
                portals[i].Render(recursionStep);
            }
        }

        for (int i = 0; i < portals.Length; i++) {
            portals[i].PostPortalRender ();
        }

    }

}