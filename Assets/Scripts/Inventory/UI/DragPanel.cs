using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragPanel : MonoBehaviour , IDragHandler
{
    RectTransform rectTransform;

    Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = InventoryManager.Instance.GetComponent<Canvas>();
    }
    public void OnDrag(PointerEventData eventData)
    {
        //通过中心点实现整体拖拽/-canvas.scaleFactor修复鼠标增量与Canvas分辨率的差异
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;  //eventData.delta 上次更新依赖的指针增量

    }

    
}
