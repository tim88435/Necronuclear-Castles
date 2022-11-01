using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager
{
    public enum GameState
    {
        MainMenu,
        MainGame,
        PauseMenu,
        Lobby,
    }
    public GameState CurrentGameState { get; private set; } = GameState.MainMenu;
    public void ChangeScene(int sceneNumber)
    {
        CurrentGameState = GameState.MainMenu;
    }
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#endif
    }
}
