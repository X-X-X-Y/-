using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuestShowTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private ItemUI currentItemUI;           //需要拿到自身UI数据

    private void Awake()
    {
        currentItemUI = GetComponent<ItemUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)          //鼠标进入获取当前物品信息
    {
        QuestUI.Instance.tooTip.gameObject.SetActive(true);
        QuestUI.Instance.tooTip.SetupTooltip(currentItemUI.currentItemData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        QuestUI.Instance.tooTip.gameObject.SetActive(false);
    }
}
