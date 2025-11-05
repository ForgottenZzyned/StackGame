using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class NamedColor
{
    public string name; 
    public Color color;
    public int combinationMult;
}

[CreateAssetMenu(menuName = "Combinations/Color Combination")]
public class ColorCombination : ScriptableObject
{
    public NamedColor resultColor;
    public List<string> inputColors;
}
