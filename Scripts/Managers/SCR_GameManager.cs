using UnityEngine;

public class SCR_GameManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject birdsEyeCamUI;
    [SerializeField] private GameObject playerUI;
    private bool bIsPaused = false;

    private bool bPlayerHasSpawned = false;

    [SerializeField] private Camera[] birdsEyeCams;
    private int camIndex = 0;
    private Camera playerCam;
    private GameObject player;

    private bool bPlayerCamIsActive = true;

    [SerializeField] private GameObject terrainMesh;

    /// <summary>
    /// If the player is using the Birds Eye View cam then you can switch between several of them
    /// Use Q and E for switching
    /// </summary>
    private void Update()
    {
        if(!bPlayerCamIsActive && bPlayerHasSpawned)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (camIndex < birdsEyeCams.Length - 1)
                {
                    birdsEyeCams[camIndex].enabled = false;
                    camIndex++;
                    birdsEyeCams[camIndex].enabled = true;
                }

                if (camIndex >= birdsEyeCams.Length - 1)
                {
                    birdsEyeCams[camIndex].enabled = false;
                    camIndex = 0;
                    birdsEyeCams[camIndex].enabled = true;
                }
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (camIndex > 0)
                {
                    birdsEyeCams[camIndex].enabled = false;
                    camIndex--;
                    birdsEyeCams[camIndex].enabled = true;
                }

                if (camIndex <= 0)
                {
                    birdsEyeCams[birdsEyeCams.Length - 1].enabled = false;
                    camIndex = birdsEyeCams.Length - 1;
                    birdsEyeCams[camIndex].enabled = true;
                }
            }
        }
    }

    /// <summary>
    /// Pauses the game to allow the user to exit to menu or to desktop
    /// Locks and/or unlocks the cursor
    /// Disables the player controls when paused
    /// </summary>
    /// <param name="bPauseState">Keeps track of the pause state</param>
    private void PauseGame(bool bPauseState)
    {
        if (bPauseState && bPlayerHasSpawned && bPlayerCamIsActive)
        {
            bIsPaused = true;
            pauseUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            SCR_CustomEvents.Player.OnSetPlayerMovement?.Invoke(false);
            SCR_CustomEvents.Player.OnSetPlayerLookState?.Invoke(false);
        }
        if (!bPauseState && bPlayerHasSpawned && bPlayerCamIsActive)
        {
            bIsPaused = false;
            pauseUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            SCR_CustomEvents.Player.OnSetPlayerMovement?.Invoke(true);
            SCR_CustomEvents.Player.OnSetPlayerLookState?.Invoke(true);
        }
    }

    /// <summary>
    /// Changes the cam perspective from player to birds eye view
    /// Disables and enables player controls
    /// </summary>
    private void ChangeCamPerspective()
    {
        if (!bIsPaused && bPlayerHasSpawned)
        {
            player = SCR_CustomEvents.Player.OnGetPlayerReference?.Invoke();
            if (player != null)
            {
                playerCam = player.GetComponentInChildren<Camera>();
            }

            if (bPlayerCamIsActive)
            {
                bPlayerCamIsActive = false;
                SCR_CustomEvents.Player.OnSetPlayerMovement?.Invoke(false);
                SCR_CustomEvents.Player.OnSetPlayerLookState?.Invoke(false);
                playerCam.enabled = false;
                birdsEyeCams[camIndex].enabled = true;
                birdsEyeCamUI.SetActive(true);
                playerUI.SetActive(false);
                if (player != null)
                {
                    player.transform.parent = terrainMesh.transform;
                }
            }
            else
            {
                bPlayerCamIsActive = true;
                SCR_CustomEvents.Player.OnSetPlayerMovement?.Invoke(true);
                SCR_CustomEvents.Player.OnSetPlayerLookState?.Invoke(true);
                playerCam.enabled = true;
                birdsEyeCamUI.SetActive(false);
                playerUI.SetActive(true);
                ResetBirdsEyeCams();
                if (player != null)
                {
                    player.transform.parent = null;
                }
            }
        }
    }

    /// <summary>
    /// No longer needed, was planned for the reset of the mesh rotation
    /// </summary>
    private void ResetMeshRotation()
    {
        terrainMesh.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
    
    /// <summary>
    /// Sets and keeps track of whether the player has spawned in
    /// </summary>
    /// <param name="state"></param>
    private void SetPlayerSpawnState(bool state)
    {
        bPlayerHasSpawned = state;
    }

    /// <summary>
    /// Returns the current pause state
    /// </summary>
    /// <returns>Pause state</returns>
    private bool GetPauseState()
    {
        return bIsPaused;
    }

    /// <summary>
    /// Sets the pause state to another value
    /// </summary>
    /// <param name="state">Sets the pause state this value</param>
    private void SetPauseState(bool state)
    {
        bIsPaused = state;
    }

    /// <summary>
    /// When the player exits the birds eye cams it resets all cams by turning them all off
    /// </summary>
    private void ResetBirdsEyeCams()
    {
        for (int i = 0; i < birdsEyeCams.Length; i++)
        {
            birdsEyeCams[i].enabled = false;
        }
    }

    private void OnEnable()
    {
        SCR_CustomEvents.Menus.OnPauseGame += PauseGame;
        SCR_CustomEvents.Player.OnSetPlayerSpawnState += SetPlayerSpawnState;
        SCR_CustomEvents.Menus.OnGetPauseState += GetPauseState;
        SCR_CustomEvents.Cameras.OnChangeCamPerspective += ChangeCamPerspective;
        SCR_CustomEvents.Menus.OnSetPauseState += SetPauseState;
        SCR_CustomEvents.Cameras.OnResetTerrainRotation += ResetMeshRotation;

    }

    private void OnDisable()
    {
        SCR_CustomEvents.Menus.OnPauseGame -= PauseGame;
        SCR_CustomEvents.Player.OnSetPlayerSpawnState -= SetPlayerSpawnState;
        SCR_CustomEvents.Menus.OnGetPauseState -= GetPauseState;
        SCR_CustomEvents.Cameras.OnChangeCamPerspective -= ChangeCamPerspective;
        SCR_CustomEvents.Menus.OnSetPauseState -= SetPauseState;
        SCR_CustomEvents.Cameras.OnResetTerrainRotation -= ResetMeshRotation;
    }
}
