using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BGManager : MonoBehaviour
{
    public static BGManager Instance;
    public Material bgMat;
    public float shaderSpeed = 0.05f;
    public Color shaderColor;
    public Color defShaderColor;
    private Sequence colorSequence;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        bgMat.DOColor(defShaderColor, "_texColor", 0.1f);
    }
    public void SetColor(Color color,float dur,Ease ease)
    {
        if (colorSequence == null)
        {
            colorSequence = DOTween.Sequence();
        }
        colorSequence.Append(bgMat.DOColor(color, "_texColor", dur).SetEase(ease));
    }

    public void SetDefColor(float dur, Ease ease)
    {
        if (colorSequence == null)
        {
            colorSequence = DOTween.Sequence();
        }
        colorSequence.Append(bgMat.DOColor(defShaderColor, "_texColor", dur).SetEase(ease));
    }
}
