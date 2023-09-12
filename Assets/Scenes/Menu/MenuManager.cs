using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Toggle = UnityEngine.UI.Toggle;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Toggle toggleFrames;
    public static bool FramesIsOn;
    [SerializeField] private TMP_InputField inputRecursionLimit;
    public static int RecursionLimit = 3;
    [SerializeField] private Toggle toggleTorus;
    public static bool TorusIsOn = true;
    [SerializeField] private Toggle toggleKleinBottle;
    public static bool KleinBottleIsOn;
    [SerializeField] private  Toggle toggleDungeonsLandscape;
    public static bool DungeonsLandscapeIsOn = true;
    [FormerlySerializedAs("toggSpaceleLandscape")] [SerializeField] private Toggle toggleSpaceLandscape;
    public static bool SpaceLandscapeIsOn;
    [SerializeField] private Toggle toggleVillageLandscape;
    public static bool VillageLandscapeIsOn;
    [SerializeField] private Toggle toggleSimpleLandscape;
    public static bool SimpleLandscapeIsOn;

    private void Start()
    {
        toggleFrames.isOn = FramesIsOn;
        inputRecursionLimit.text = RecursionLimit.ToString();
        toggleTorus.isOn = TorusIsOn;
        toggleKleinBottle.isOn = KleinBottleIsOn;
        toggleDungeonsLandscape.isOn = DungeonsLandscapeIsOn;
        toggleSpaceLandscape.isOn = SpaceLandscapeIsOn;
        toggleVillageLandscape.isOn = VillageLandscapeIsOn;
        toggleSimpleLandscape.isOn = SimpleLandscapeIsOn;

    }

    public void StartVisualisation()
    {
        RecursionLimit = Convert.ToInt32(inputRecursionLimit.text);
        FramesIsOn = toggleFrames.isOn;
        TorusIsOn = toggleTorus.isOn;
        KleinBottleIsOn = toggleKleinBottle.isOn;
        DungeonsLandscapeIsOn = toggleDungeonsLandscape.isOn;
        SpaceLandscapeIsOn = toggleSpaceLandscape.isOn;
        VillageLandscapeIsOn = toggleVillageLandscape.isOn;
        SimpleLandscapeIsOn = toggleSimpleLandscape.isOn;
        
        if (toggleTorus.isOn)
        {
            SceneManager.LoadScene("TorusScene");
        }

        if (toggleKleinBottle.isOn)
        {
            SceneManager.LoadScene("KleinScene");
        }
    }

    public void ExitVisualisation()
    {
        Application.Quit();
    }
}
