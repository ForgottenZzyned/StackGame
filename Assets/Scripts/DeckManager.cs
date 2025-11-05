using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;
    public List<ColorInfo> currentDeck = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        AddStartDeck(10,10,10);
    }

    public void AddStartDeck(int redCount,int blueCount,int greenCount)
    {
        AddColorToDeck(new ColorInfo(ColorRank.Medium, ColorType.Red, Color.red),redCount);
        AddColorToDeck(new ColorInfo(ColorRank.Medium, ColorType.Blue, Color.blue),blueCount);
        AddColorToDeck(new ColorInfo(ColorRank.Medium, ColorType.Green, Color.green),greenCount);
    }
    public void AddColorToDeck(ColorInfo colorToAdd, int count = 1)
    {
        for(var i = 0; i< count;i++)
        {
            currentDeck.Add(colorToAdd);
        }
    }

    public void RemoveColorFromDeck(ColorInfo colorToRemove)
    {
        if (currentDeck.Contains(colorToRemove))
        {
            currentDeck.Remove(colorToRemove);
        }
    }

    public List<ColorInfo> SetColorConveyor()
    {
        ColorInfo[] conveyorList = new ColorInfo[currentDeck.Count];
        foreach (var color in currentDeck)
        {
            int index = Random.Range(0, currentDeck.Count);
            for (var i = 0; i < currentDeck.Count; i++)
            {
                if (conveyorList[index] != null) index += 1;
                if (index >= conveyorList.Length) index = 0;
                if (conveyorList[index] == null) break;
            }
            conveyorList[index] = color;
        }
        return conveyorList.ToList();
    }
}
