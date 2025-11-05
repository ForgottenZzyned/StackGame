using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class MainMenuManager : MonoBehaviour
{
    public List<GameObject> gameNameTextLetters;

    private void Start()
    {
        StartCoroutine(StartAnimations());
    }

    private IEnumerator StartAnimations()
    {
        while (true)
        {
            AnimateName();
            yield return new WaitForSeconds(15f);
        }

        yield return null;
    }
    private void AnimateName()
    {
        foreach (var letter in gameNameTextLetters)
        {
            
        }
    }
}
