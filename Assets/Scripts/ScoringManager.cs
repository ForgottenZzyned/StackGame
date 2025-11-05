using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.XR;

public enum GameState
{
    SelectingCards,
    Scoring,
    Shopping,
    Waiting
}
public class ScoringManager : MonoBehaviour
{
    public static ScoringManager Instance;
    public int handsRemained = 0;
    public int selectedAmount = 0;
    public int maxSelectedAmount = 0;
    public int attemptsRemained = 0;
    public float totalColVol = 0;
    public TMP_Text totalColVolText;
    public float totalScore = 0;
    public TMP_Text totalScoreText;
    public float totalMult = 0;
    public TMP_Text multText;
    public TMP_Text resultCombinationText;
    public TMP_Text combinationText;

    public List<string> inputColors = new();

    public AudioClip scoringSound;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public IEnumerator HandInAttempt()
    {
        List<DraggableCard> selectedCards = GetSelectedCards(StackPanelManager.Instance.objects);
        if (selectedCards == null) yield break;
        RoundManager.Instance.gameState = GameState.Scoring;
        yield return StartCoroutine(ScoreHand(selectedCards));
        handsRemained--;
        attemptsRemained--;
        if(handsRemained > 0 || attemptsRemained > 0)RoundManager.Instance.gameState = GameState.SelectingCards;
        else RoundManager.Instance.gameState = GameState.Waiting;
    }
    public void HandInButton()
    {
        if (RoundManager.Instance.gameState == GameState.Scoring) return;
        StartCoroutine(HandInAttempt());
    }
    private void CheckAttempts()
    {

    }

    private IEnumerator ScoreHand(List<DraggableCard> hand)
    {
        float timeBetweenScoring = 0.5f;
        foreach (DraggableCard card in hand)
        {
            card.enabled = false;
        }
        foreach (DraggableCard card in hand)
        {
            var cardInfo = card.GetComponent<CardScript>();
            ScoreCard(cardInfo);
            yield return new WaitForSeconds(timeBetweenScoring);
            if(timeBetweenScoring > 0.30f)timeBetweenScoring -= 0.05f;
            else if(timeBetweenScoring > 0.05f)timeBetweenScoring -= 0.01f;
        }
        yield return new WaitForSeconds(0.25f);
        NamedColor result = GetCombination(inputColors);
        totalScore += totalColVol * totalMult * result.combinationMult;
        totalScoreText.text = "Score: " + totalScore.ToString();
        totalColVol = 0f;
        totalMult = 0f;
        totalColVolText.text = "Color Power: " + totalColVol.ToString();
        multText.text = "Mult: " + totalMult.ToString();
        TextVibro(totalColVolText.gameObject,0.2f, 15);
        TextVibro(multText.gameObject, 0.2f, 15);
        TextVibro(totalScoreText.gameObject, 0.2f, 15);
        DeselectCards(hand);
        UseCombinationEffects(result);
        yield return new WaitForSeconds(0.7f);
        BGManager.Instance.SetDefColor(1f, Ease.InBack);
        foreach (DraggableCard card in hand)
        {
            card.enabled = true;
        }
        yield return null;
    }

    private void UseCombinationEffects(NamedColor result)
    {
        resultCombinationText.text = result.name + $"\n x{result.combinationMult} Score!!!";
        Color col = Color.white;
        col.a = 0f;
        resultCombinationText.color = col;
        Color endCol = result.color;
        endCol.a = 0f;
        BGManager.Instance.SetColor(result.color, 0.7f, Ease.OutBack);
        Sequence fade = DOTween.Sequence();
        TextVibro(resultCombinationText.gameObject, 0.4f,20);
        fade.Append(resultCombinationText.DOColor(result.color, 0.15f))
            .AppendInterval(1f)
            .Append(resultCombinationText.DOColor(endCol, 1f));
    }
    private static NamedColor GetCombination(List<string> inputCols)
    {
        var result = CombinationManager.Instance.GetCombinationResult(inputCols);
        return result;
    }
    private void ScoreCard(CardScript cardInfo)
    {
        float endColVol = totalColVol + cardInfo.info.colorVolAmount;
        float endMult = totalMult + cardInfo.info.multAmount;
        totalColVolText.text = "Color Power: "+ endColVol.ToString();
        cardInfo.gameObject.GetComponentInChildren<AnimCardScript>().PlayResonance();
        //SFXManager.Instance.PlaySFX(scoringSound,Random.Range(0.9f,1.1f));
        TextVibro(totalColVolText.gameObject, 0.2f, 15);
        multText.text = "Mult: "+endMult.ToString();
        TextVibro(multText.gameObject, 0.2f, 15);
        totalColVol = endColVol;
        totalMult = endMult;
    }

    public void DeselectCards(List<DraggableCard> cards)
    {
        foreach (DraggableCard card in cards)
        {
            card.gameObject.GetComponentInChildren<AnimCardScript>().SwitchSelect();
        }
    }

    private void TextVibro(GameObject objToVibro, float dur, float rotStrength)
    {
        Sequence rotS = DOTween.Sequence();
        Sequence scale = DOTween.Sequence();
        scale.Append(objToVibro.transform.DOScale(0.8f, dur).SetEase(Ease.OutBack))
            .AppendInterval(dur)
            .Append(objToVibro.transform.DOScale(1.2f, dur).SetEase(Ease.OutBack));
        rotS.Append(objToVibro.transform.DORotate(new Vector3(0, 0, rotStrength), 0.04f))
            .Join(objToVibro.transform.DOLocalMoveZ(-0.05f, 0.04f))
            .Append(objToVibro.transform.DORotate(new Vector3(0, 0, -rotStrength), 0.04f))
            .Join(objToVibro.transform.DOLocalMoveZ(0.05f, 0.04f))
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
    private List<DraggableCard> GetSelectedCards(List<DraggableCard> cardList)
    {
        List<DraggableCard> temp = new();
        foreach (DraggableCard card in cardList)
        {
            if (card.GetComponent<CardScript>().selected)
            {
                temp.Add(card);
            }
        }
        if(temp.Count > 0) return temp;
        return null;
    }

    public void ChangeInputColor(bool add, string colorName)
    {
        if (add)
        {
            inputColors.Add(colorName);
        }
        else
        {
            if (inputColors.Contains(colorName)) inputColors.Remove(colorName);
        }
        var likelyCombination = CombinationManager.Instance.GetCombinationResult(inputColors, true);
        SetCombinationText(likelyCombination);
    }
    public void SetCombinationText(NamedColor info)
    {
        if (info == null)
        {
            combinationText.text = "";
            combinationText.color = Color.white;
            return;
        }
        if (combinationText.text == "Most Likely " + info.name + $"\n x{info.combinationMult}\n ") return;
        combinationText.text = "Most Likely "+info.name + $"\n x{info.combinationMult}\n ";
        TextVibro(combinationText.gameObject, 0.1f, 2f);
        combinationText.color = info.color;
    }
}
