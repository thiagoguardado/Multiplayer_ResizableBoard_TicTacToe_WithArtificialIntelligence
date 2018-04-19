using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Menu,
    GameStarted,
    GameFinished
}

public class GameManager : MonoBehaviour {

    private static GameState gameState = GameState.Menu;
    public static GameState GameState
    {
        get
        {
            return gameState;
        }
    }

    public static bool playerCanInteract = true;

}
