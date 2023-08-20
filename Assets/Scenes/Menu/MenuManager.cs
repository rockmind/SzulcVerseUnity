using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Toggle = UnityEngine.UI.Toggle;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Toggle toggleFrames;
    public static bool Frames = false;
    [SerializeField] private TMP_InputField inputRecursionLimit;
    public static int RecursionLimit = 3;
    [SerializeField] private Toggle toggleTorus;
    public static bool Torus = true;
    [SerializeField] private Toggle toggleKleinBottle;
    public static bool KleinBottle = false;
    [SerializeField] private Toggle toggleLandscape1;
    [SerializeField] private Toggle toggleLandscape2;
    [SerializeField] private Toggle toggleLandscape3;

    private void Start()
    {
        toggleFrames.isOn = Frames;
        inputRecursionLimit.text = RecursionLimit.ToString();
        toggleTorus.isOn = Torus;
        toggleKleinBottle.isOn = KleinBottle;

    }

    public void StartVisualisation()
    {
        RecursionLimit = Convert.ToInt32(inputRecursionLimit.text);
        Frames = toggleFrames.isOn;
        Torus = toggleTorus.isOn;
        KleinBottle = toggleKleinBottle.isOn;
        
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
