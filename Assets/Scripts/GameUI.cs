using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IGameUI
{
    void RefreshScore();
    void OnClickRestart();
    event OnClickRestartEventHandler OnClickRestartEvent;

}

public delegate void OnClickRestartEventHandler();

public class GameUI : MonoBehaviour, IGameUI
{

    public event OnClickRestartEventHandler OnClickRestartEvent;
    public Button button;

    private void Start()
    {
        button.onClick.AddListener(()=>OnClickRestart());
    }


    public void RefreshScore()
    {

    }


    public void OnClickRestart()
    {





        OnClickRestartEvent?.Invoke();
    }



}
