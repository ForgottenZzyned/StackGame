using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnimCardScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image image;
    public Color baseColor;
    public Color resonanceColor;
    public Color highLightedColor;

    public CardScript cardScript;
    public Material colorMat;

    private DraggableCard dragComponent;

    public Quaternion originalRot;
    public Vector2 originalPos;
    private void Awake()
    {
        cardScript = GetComponentInParent<CardScript>();
        dragComponent = GetComponentInParent<DraggableCard>();
        colorMat = new Material(GetComponent<Image>().material);
        GetComponent<Image>().material = colorMat;
    }
    private void Start()
    {
        originalRot = transform.rotation;
        originalPos = transform.position;
        colorMat.SetColor("_baseColor", baseColor);
    }
    public void SetOrigPos(Vector3 pos)
    {
        originalPos = pos;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        image.DOColor(highLightedColor, 0.2f);
        transform.DOScale(1.1f, 0.2f);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        image.DOColor(baseColor, 0.2f);
        transform.DOScale(1f, 0.2f);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(colorMat);
        colorMat.SetFloat("_DestroyedFloat", 0f);
        colorMat.DOFloat(10f, "_DestroyedFloat", 2f);
        //PlaySwitchResonance();
    }
    public void SwitchSelect()
    {
        Sequence selectS = DOTween.Sequence();
        cardScript.selected = !cardScript.selected;
        if (cardScript.selected)
        {
            ScoringManager.Instance.selectedAmount++;
            selectS.Append(transform.DOMove(transform.position + transform.up * 1f, 0.25f).SetEase(Ease.OutBounce));
            ScoringManager.Instance.ChangeInputColor(true,cardScript.info.type.ToString());
        }
        else
        {
            ScoringManager.Instance.selectedAmount--;
            selectS.Append(transform.DOMove(originalPos, 0.25f).SetEase(Ease.OutBounce));
            ScoringManager.Instance.ChangeInputColor(false, cardScript.info.type.ToString());
        }
    }
    public void PlaySwitchResonance()
    {
        if (RoundManager.Instance.gameState == GameState.Scoring) return;
        if (ScoringManager.Instance.selectedAmount >= ScoringManager.Instance.maxSelectedAmount && !cardScript.selected) return;
        if (dragComponent.dragged) return;
        Color c = image.color;
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOScale(1.2f, 0.2f))
            .AppendCallback(SwitchSelect)
            .Join(image.DOColor(resonanceColor, 0.2f))
           .Join(transform.DOShakePosition(0.4f, 10f, 20, 90))
           .Append(transform.DOScale(1f, 0.2f))
           .Join(image.DOColor(baseColor, 0.2f))
           .SetEase(Ease.OutBack)
           .Append(transform.DORotate(new Vector3(originalRot.x, originalRot.y, originalRot.z), 0.04f));
        Invoke(nameof(SetOrigPos),0.5f);
    }
    public void PlayResonance()
    {
        if (dragComponent.dragged) return;
        Color c = image.color;
        Sequence rotS = DOTween.Sequence();

        rotS.Append(transform.DORotate(new Vector3(0, 0, 8f), 0.04f))
            .Join(transform.DOLocalMoveZ(-0.05f, 0.04f))
            .Append(transform.DORotate(new Vector3(0, 0, -8f), 0.04f))
            .Join(transform.DOLocalMoveZ(0.05f, 0.04f))
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .Pause();
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOScale(1.2f, 0.2f))
            .Join(image.DOColor(resonanceColor, 0.2f))
            .AppendCallback(() => rotS.Restart())
            .Append(transform.DOScale(1f, 0.2f))
            .Join(image.DOColor(baseColor, 0.2f))
            .SetEase(Ease.OutBack)
            .Append(transform.DORotate(new Vector3(originalRot.x, originalRot.y, originalRot.z), 0.04f));
    }
}
