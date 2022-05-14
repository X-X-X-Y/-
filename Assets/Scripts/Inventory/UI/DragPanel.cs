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
        //ͨ�����ĵ�ʵ��������ק/-canvas.scaleFactor�޸����������Canvas�ֱ��ʵĲ���
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;  //eventData.delta �ϴθ���������ָ������

    }

    
}
