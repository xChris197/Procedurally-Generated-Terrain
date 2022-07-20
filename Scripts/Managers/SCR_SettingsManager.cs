using UnityEngine;
using TMPro;

public class SCR_SettingsManager : MonoBehaviour
{
    [SerializeField] private SCR_MeshGenerator meshGen;
    
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject generationUI;
    [SerializeField] private GameObject playerUI;

    [SerializeField] private TMP_InputField seedInputField;
    
    [SerializeField] private Camera uiCam;
    
    /// <summary>
    /// Sets the player spawn state to false
    /// Turns off all other UI and turns on the menus UI
    /// Resets the mesh terrain to be empty
    /// </summary>
    public void QuitToMenu()
    {
        meshGen.ClearTerrain();
        SCR_CustomEvents.Player.OnSetPlayerSpawnState?.Invoke(false);
        SCR_CustomEvents.Menus.OnSetPauseState?.Invoke(false);
        seedInputField.text = "";
        pauseUI.SetActive(false);
        playerUI.SetActive(false);
        generationUI.SetActive(true);
        uiCam.enabled = true;
        SCR_CustomEvents.Cameras.OnResetTerrainRotation?.Invoke();
    }
    
    /// <summary>
    /// Exits the player to desktop
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
