using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class StackPanelManager : MonoBehaviour
{
    public static StackPanelManager Instance; 
    public RectTransform panel;
    public float moveDuration = 0.2f;
    public float edgePercent = 0.15f;

    public List<DraggableCard> objects = new List<DraggableCard>();
    private float totalWidth;
    private float edgeOffset;
    private float spacing;

    private Camera cam;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        Canvas canvas = panel.GetComponentInParent<Canvas>();
        cam = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : Camera.main;
        RecalculateLayout();
    }
    public void RegisterObject(DraggableCard obj)
    {
        if (!objects.Contains(obj))
            objects.Add(obj);
        RecalculateLayout();
    }

    public void UnregisterObject(DraggableCard obj)
    {
        if (objects.Contains(obj))
            objects.Remove(obj);
        RecalculateLayout();
    }
    private void RecalculateLayout()
    {
        if (objects.Count == 0) return;

        Vector3[] corners = new Vector3[4];
        panel.GetLocalCorners(corners);
        totalWidth = corners[3].x - corners[0].x;
        edgeOffset = totalWidth * edgePercent;
        float availableWidth = totalWidth - edgeOffset * 2;
        spacing = availableWidth / Mathf.Max(objects.Count - 1, 1);
    }

    public void UpdatePositions(DraggableCard draggedObj = null, Vector3? dragPos = null)
    {
        if (objects.Count == 0) return;
        List<DraggableCard> temp = new List<DraggableCard>(objects);
        if (draggedObj != null)
            temp.Remove(draggedObj);
        int targetIndex = temp.Count;
        if (dragPos.HasValue)
        {
            float localX = panel.InverseTransformPoint(dragPos.Value).x;
            float slotX0 = -totalWidth / 2f + edgeOffset;
            targetIndex = Mathf.Clamp(Mathf.RoundToInt((localX - slotX0) / spacing), 0, temp.Count);
        }
        for (int i = 0, j = 0; i < temp.Count; i++, j++)
        {
            if (j == targetIndex) j++;
            Vector3 localTarget = new Vector3(-totalWidth / 2f + edgeOffset + spacing * j, 0, 0);
            Vector3 worldTarget = panel.TransformPoint(localTarget);
            temp[i].transform.DOMove(worldTarget, moveDuration).SetEase(Ease.OutQuad);
            temp[i].GetComponentInChildren<AnimCardScript>().SetOrigPos(worldTarget);
        }
    }

    public void DropObject(DraggableCard obj, Vector3 dropPos)
    {
        float localX = panel.InverseTransformPoint(dropPos).x;
        float slotX0 = -totalWidth / 2f + edgeOffset;
        int targetIndex = Mathf.Clamp(Mathf.RoundToInt((localX - slotX0) / spacing), 0, objects.Count - 1);

        objects.Remove(obj);
        objects.Insert(targetIndex, obj);
        UpdatePositions(); 
    }
}
