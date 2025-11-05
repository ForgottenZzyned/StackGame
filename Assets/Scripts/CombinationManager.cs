using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CombinationManager : MonoBehaviour
{
    public static CombinationManager Instance;
    [SerializeField] private ColorCombinationManager combManager;
    private List<ColorCombination> combinations = new();

    private void Awake()
    {
        combinations.Clear();
        foreach (var combo in combManager.allCombinations)
        {
            combinations.Add(combo);
        }
        if(Instance == null) Instance = this;
    }

    public NamedColor GetCombinationResult(List<string> inputColors, bool guaranteed = false)
    {
        if (inputColors.Count <= 0) return null; 
        if (inputColors.All(c => c == inputColors[0]))
        {
            return new NamedColor
            {
                name = inputColors[0],
                color = GetColorByName(inputColors[0]),
                combinationMult = 1
            };
        }

        var sortedInput = inputColors.OrderBy(c => c).ToList();
        foreach (var combo in combinations)
        {
            var sortedCombo = combo.inputColors.OrderBy(c => c).ToList();
            if (sortedCombo.SequenceEqual(sortedInput))
            {
                if (guaranteed)
                {
                    return combo.resultColor;
                }
                else
                {
                    return combo.resultColor;
                    //return GetRandomResult(combo.possibleResults);
                }
            }
        }
        return GetClosestCombination(sortedInput, guaranteed);
    }

    private NamedColor GetClosestCombination(List<string> inputColors, bool guaranteed = false)
    {
        Dictionary<ColorCombination, float> similarityMap = new();

        foreach (var combo in combinations)
        {
            int matches = combo.inputColors.Count(c => inputColors.Contains(c));
            if (matches > 0)
            {
                float similarity = (float)matches / combo.inputColors.Count;
                similarityMap[combo] = similarity;
            }
        }

        if (similarityMap.Count == 0)
            return new NamedColor { name = "Unknown" };
        var bestCombo = similarityMap.OrderByDescending(kvp => kvp.Value).First().Key;

        if (guaranteed)
        {
            return bestCombo.resultColor;
            //return bestCombo.possibleResults.OrderByDescending(r => r.chance).First().resultColor;
        }
        else
        {
            return bestCombo.resultColor;
            //return GetRandomResult(bestCombo.possibleResults);
        }
    }
    
    /*private NamedColor GetRandomResult(List<CombinationResult> results)
    {
        float totalChance = results.Sum(r => r.chance);
        float rand = Random.Range(0f, totalChance);
        float current = 0f;

        foreach (var res in results)
        {
            current += res.chance;
            if (rand <= current)
                return res.resultColor;
        }

        return results.Last().resultColor;
    }*/
    private Color GetColorByName(string colorName)
    {
        switch (colorName)
        {
            case "Red": return Color.red;
            case "Blue": return Color.blue;
            case "Green": return Color.green;
            case "Yellow": return Color.yellow;
            case "Purple": return new Color(0.6f, 0.2f, 0.8f);
            case "Orange": return new Color(1f, 0.5f, 0f);
            default: return Color.white;
        }
    }
}
