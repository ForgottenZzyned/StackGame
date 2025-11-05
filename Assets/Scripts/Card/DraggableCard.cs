using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

interface IDraggable : IBeginDragHandler, IDragHandler, IEndDragHandler
{
   public void RotateInDir(Vector3 pos);
}

[RequireComponent(typeof(CanvasGroup))]
public class DraggableCard : MonoBehaviour, IDraggable
{
    private StackPanelManager manager;
    private Canvas canvas;
    private RectTransform rect;
    private CanvasGroup cg;
    public bool dragged;
    private Vector3 lastDragPos;
    private Quaternion startDragPos;
    public float rotationAmount = 15f;
    public float rotateSpeed = 0.2f;   

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        manager = FindObjectOfType<StackPanelManager>();
        canvas = manager.GetComponentInParent<Canvas>();
        cg = GetComponent<CanvasGroup>();
        manager.RegisterObject(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        cg.blocksRaycasts = false;
        startDragPos = transform.rotation;
        dragged = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, eventData.position, canvas.worldCamera, out worldPos);
        rect.position = worldPos;
        RotateInDir(worldPos);
        manager.UpdatePositions(this, worldPos);
    }

    public void RotateInDir(Vector3 pos)
    {
        float deltaX = pos.x - lastDragPos.x;
        float velocity = deltaX / Time.deltaTime;
        float normalized = Mathf.Clamp(velocity / 1000f, -30f, 30f);
        float targetZ = -normalized * rotationAmount;
        transform.DORotate(new Vector3(0, 0, targetZ), rotateSpeed, RotateMode.Fast);
        lastDragPos = pos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (RoundManager.Instance.gameState == GameState.Scoring) return;
        cg.blocksRaycasts = true;
        Vector3 worldPos;
        transform.DORotate(new Vector3(0, 0, startDragPos.z), rotateSpeed);
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, eventData.position, canvas.worldCamera, out worldPos);
        manager.DropObject(this, worldPos);
        dragged = false;
    }
}
