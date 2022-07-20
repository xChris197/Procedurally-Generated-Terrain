using System;
using UnityEngine;

/// <summary>
/// A collection of events used for various systems
/// Split between several classes
/// Avoids needing references to other scripts as well
/// </summary>
public class SCR_CustomEvents
{
    public class Cameras
    {
        public static Action OnChangeCamPerspective;
        public static Action OnResetTerrainRotation;
    }
    
    public class Menus
    {
        public static Action<bool> OnPauseGame;
        public static Func<bool> OnGetPauseState;
        public static Action<bool> OnSetPauseState;
    }

    public class Player
    {
        public static Action<bool> OnSetPlayerSpawnState;
        public static Action<bool> OnSetPlayerMovement;
        public static Action<bool> OnSetPlayerLookState;
        public static Func<GameObject> OnGetPlayerReference;
    }
}
