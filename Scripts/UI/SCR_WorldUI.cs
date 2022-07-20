using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SCR_WorldUI : MonoBehaviour
{
    private int worldSeed;
    [SerializeField] private TMP_InputField worldSeedInputField;

    private int regionIndex;
    [SerializeField] private TMP_Dropdown regionDropdown;

    private int propDensityIndex;
    [SerializeField] private Slider propDensitySlider;

    [SerializeField] private Camera uiCam;
    [SerializeField] private GameObject generationUI;
    [SerializeField] private GameObject playerUI;
    
    [SerializeField] private SCR_MapGenerator mapGen;

    public void GenerateWorld()
    {
        if (worldSeedInputField.text == "")
        {
            worldSeed = 0;
        }
        else
        {
            worldSeed = int.Parse(worldSeedInputField.text);
        }
        
        if (worldSeed < 0)
        {
            worldSeed = 0;
        }

        if (worldSeed > 100000)
        {
            worldSeed = 100000;
        }

        propDensityIndex = (int)propDensitySlider.value;
        
        regionIndex = regionDropdown.value;

        uiCam.enabled = false;
        generationUI.SetActive(false);
        playerUI.SetActive(true);
        mapGen.SetWorldParams(worldSeed, regionIndex, propDensityIndex);
        SCR_CustomEvents.Player.OnSetPlayerSpawnState?.Invoke(true);
    }
}
