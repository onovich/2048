using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveEventArgs : EventArgs
{
    public int dir;
    public MoveEventArgs(int dir)
    {
        this.dir = dir;
    }
}


public interface IPlayerController
{
    void Press();
    void Click();
    event EventHandler<MoveEventArgs> OnMoveEvent;
    event OnEscEventHandler OnEscEvent;
    event OnRestartEventHandler OnRestartEvent;

}


public delegate void OnEscEventHandler();
public delegate void OnRestartEventHandler();




public class PlayerController : MonoBehaviour, IPlayerController
{
    public event EventHandler<MoveEventArgs> OnMoveEvent;


    public event OnEscEventHandler OnEscEvent;
    public event OnRestartEventHandler OnRestartEvent;

    public Button button;

    MoveEventArgs e = new MoveEventArgs(0);

    public void Press()
    {

        if (Input.GetKeyDown(KeyCode.W))
        {
            e.dir = 1;
            //OnMoveUpEvent?.Invoke();
            OnMoveEvent?.Invoke(this,e);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            e.dir = 2;
            //OnMoveDownEvent?.Invoke();
            OnMoveEvent?.Invoke(this,e);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            e.dir = 3;
            //OnMoveLeftEvent?.Invoke();
            OnMoveEvent?.Invoke(this, e);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            e.dir = 4;
            //OnMoveRightEvent?.Invoke();
            OnMoveEvent?.Invoke(this, e);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscEvent?.Invoke();
        }
    }

    public void Click()
    {
        OnRestartEvent?.Invoke();
    }

    /*
    void OnMouseClickRestart()
    {
        Vector3 mousePos = Input.mousePosition;
    }
    */



}
