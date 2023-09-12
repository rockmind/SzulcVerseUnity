using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Landscape : MonoBehaviour
{
    public GameObject scenery;
    void Start()
    {
        string sceneryName = scenery.name;

        switch (sceneryName)
        {
            case "DungeonScenery":
                scenery.SetActive(MenuManager.DungeonsLandscapeIsOn);
                break;
            case "SpaceScenery":
                scenery.SetActive(MenuManager.SpaceLandscapeIsOn);
                break;
            case "VillageScenery":
                scenery.SetActive(MenuManager.VillageLandscapeIsOn);
                break;
            case "SimpleScenery" :
                scenery.SetActive(MenuManager.SimpleLandscapeIsOn);
                break;
        }
    }
}
