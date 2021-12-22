using System.Collections;
using System.Collections.Generic;
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

    }

    public void QuitGame()
    {
    }

    public void Restart()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(Level);
        op.allowSceneActivation = true;
        Debug.Log("reset done");
    }

    public void StartGame()
    {
    }

    public void Success()
    {
    }
}
