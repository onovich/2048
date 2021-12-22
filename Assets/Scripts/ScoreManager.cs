using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScoreManager
{
    void RefreshScore();
    void RefreshBest();
    void RefreshSteps();
    event OnScoreRefreshEventHandler OnScoreRefreshEvent;
}
public delegate void OnScoreRefreshEventHandler();


public class ScoreManager : MonoBehaviour, IScoreManager
{
    public event OnScoreRefreshEventHandler OnScoreRefreshEvent;

    public void RefreshBest()
    {
    }

    public void RefreshScore()
    {
    }

    public void RefreshSteps()
    {
    }
}
