using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuestShowTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private ItemUI currentItemUI;           //��Ҫ�õ�����UI����

    private void Awake()
    {
        currentItemUI = GetComponent<ItemUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)          //�������ȡ��ǰ��Ʒ��Ϣ
    {
        QuestUI.Instance.tooTip.gameObject.SetActive(true);
        QuestUI.Instance.tooTip.SetupTooltip(currentItemUI.currentItemData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        QuestUI.Instance.tooTip.gameObject.SetActive(false);
    }
}
