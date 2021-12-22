using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IGame
{
    void StartGame();
    void Success();
    void GameOver();
    void Restart();
    void QuitGame();
}

public class Game : MonoBehaviour, IGame
{

    string Level;

    private void Start()
    {
        Level = SceneManager.GetActiveScene().name;
    }

    public void Ctor(PlayerController PlayerController)
    {

    }

    public void GameOver()
    {
        QuitGame();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR //在编辑器模式下

        EditorApplication.isPlaying = false;

#else //正式环境下

Application.Quit();

#endif
    }

    public void Restart()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(Level);
        op.allowSceneActivation = true;
        Debug.Log("游戏重启");
    }

    public void StartGame()
    {
    }

    public void Success()
    {
    }
}
