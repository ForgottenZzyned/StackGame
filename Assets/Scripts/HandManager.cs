using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HandManager : MonoBehaviour
{
    public List<CardScript> currentHand;
    public List<ColorInfo> deckSequence;
    public GameObject colorPref;
    public GameObject parentCanv;
    public Transform colorSpawnPos;
    public int handSize = 10;

    private void Start()
    {
        
    }

    public void TestButton()
    {
        StartCoroutine(SetStartHand());
    }
    public IEnumerator SetStartHand()
    {
        deckSequence = DeckManager.Instance.SetColorConveyor();
        for (var i = 0; i < handSize - currentHand.Count; i++)
        {
            AddColorToHand(deckSequence[0]);
            yield return new WaitForSeconds(0.1f);
        }
        StackPanelManager.Instance.UpdatePositions();
    }
    public void DiscardHand()
    {

    }
    public void AddColorToHand(ColorInfo colorInfo)
    {
        GameObject colorObject = Instantiate(colorPref, colorSpawnPos.position, Quaternion.identity,parentCanv.transform);
        CardScript colorScript = colorObject.GetComponent<CardScript>();
        colorScript.SerializeCard(colorInfo);
        StackPanelManager.Instance.UpdatePositions();
        deckSequence.Remove(colorInfo);
    }
    public void DiscardColorFromHand()
    {

    }

}
