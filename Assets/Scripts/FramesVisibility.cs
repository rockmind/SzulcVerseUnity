using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramesVisibility : MonoBehaviour
{
    public GameObject frames;
    void Start()
    {
        frames.SetActive(MenuManager.FramesIsOn);
    }
}
