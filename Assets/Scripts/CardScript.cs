using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ColorType
{
    Red,
    Green,
    Blue
}

public enum ColorRank
{
    Little,
    Medium,
    Large,
    Mega,
    Ultra
}
[System.Serializable]
public class ColorInfo
{
    public ColorType type;
    public ColorRank rank;
    public Color baseColor;
    public Color resonanceColor;
    public Color highlightedColor;
    public float colorVolAmount = 5;
    public float multAmount = 5;
    public ColorInfo(ColorRank rank, ColorType type, Color baseColor)
    {
        this.rank = rank;
        this.type = type;
        this.baseColor = baseColor;
        Color.RGBToHSV(baseColor, out float h, out float s, out float v);
        v = Mathf.Clamp01(v + 0.2f); 
        this.highlightedColor = Color.HSVToRGB(h, s, v);
        this.colorVolAmount = 5;
        this.multAmount = 5;
    }
}
public class CardScript : MonoBehaviour
{
    public ColorInfo info;
    public bool selected = false;
    public void DeleteCard()
    {
        Destroy(gameObject);
    }
    public void SerializeCard(ColorInfo info)
    {
        this.info = info;
        AnimCardScript animComponent = GetComponentInChildren<AnimCardScript>();
        animComponent.baseColor = info.baseColor;
        animComponent.image.color = info.baseColor;
        animComponent.highLightedColor = info.highlightedColor;
    }
}
