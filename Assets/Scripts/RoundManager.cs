using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;
    public GameState gameState = GameState.Waiting;
    public int currentRound = 0;

    private void Awake()
    {
        if(Instance == null) Instance = this;
    }

    public void StartRound()
    {

    }
    public void EndRound()
    {

    }
}
