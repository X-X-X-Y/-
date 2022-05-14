using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemUI))]          //���������������ȷ���ܹ���ȡ
public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //ʹ�ýӿ�ʵ�ֱ�������ק���� EventSystem

    ItemUI currentItemUI;       //��ȡItemUI�е�ǰ��Ʒ��Ϣ
    SlotHolder currentHolder;   //ͨ���м��߽�������-��ȡ��ǰ����/Ŀ�������Ϣ
    SlotHolder targetHolder;

    private void Awake()
    {
        //��ȡ��ǩ���Ӽ���Ʒ��Ϣ
        currentItemUI = GetComponent<ItemUI>();
        currentHolder = GetComponentInParent<SlotHolder>();
    }

    public void OnBeginDrag(PointerEventData eventData)     //PointerEventData-��ָ��/���/���� �¼���������Ч����
    {
        InventoryManager.Instance.currentDrag = new InventoryManager.DragData();                //��ֵ��ק��Ʒ������
        InventoryManager.Instance.currentDrag.originalHolder = GetComponent<SlotHolder>();
        InventoryManager.Instance.currentDrag.originalParent = (RectTransform)transform.parent;
        

        //�����-��ʼ��ק-��¼ԭʼ��Ϣ
        transform.SetParent(InventoryManager.Instance.dragCanvas.transform,true);   //��ʼ��קʱ��������Ⱦ���ó�dragCanvas

    }

    public void OnDrag(PointerEventData eventData)
    {
        //��ק��-��������ƶ�
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //�ɿ����-������ק-������Ʒ-������ק
        
        if (EventSystem.current.IsPointerOverGameObject())                  //��קָ���Ƿ�ΪUI����ϵIsPointerOverGameObject-���������ֵ�UI�ֱ���
        {
            if (InventoryManager.Instance.CheckActionUI(eventData.position) ||
                InventoryManager.Instance.CheckEquipmentUI(eventData.position)||
                InventoryManager.Instance.CheckInInventoryUI(eventData.position))
            {
                //��Ϊ��ק��ʱ����ק����Ʒ�ᱻ�Ž���ʾ��Canvas
                if (eventData.pointerEnter.gameObject.GetComponent<SlotHolder>())   //�ж�������ĸ���������û��SlotHolder
                    targetHolder = eventData.pointerEnter.gameObject.GetComponent<SlotHolder>(); //����ǰ��SlotHolder����Ŀ��
                else
                    targetHolder = eventData.pointerEnter.gameObject.GetComponentInParent<SlotHolder>();    //ȥ������SlotHolder

                //�ж��Ƿ���ԭ�ȵ�Hold
                if (targetHolder != InventoryManager.Instance.currentDrag.originalHolder)
                    switch (targetHolder.slotType)          //�жϵ��߽���ָ���ĸ���
                {
                    case SlotType.BAG:
                        SwapItem();
                        break;
                    case SlotType.WEAPON:
                        if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Weapon)
                            SwapItem();
                        break;
                    case SlotType.ARMOR:
                        if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Armor)
                            SwapItem();
                        break;
                    case SlotType.ACTION:
                        if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Useable)     //ͨ��UI�жϵ�ǰ��ק��Ʒ������
                            SwapItem();
                        break;
                }

                currentHolder.UpdateItem();
                targetHolder.UpdateItem();
            }
        }
        transform.SetParent(InventoryManager.Instance.currentDrag.originalParent);        //��ק�ĸ��ӽ�����ص��Լ��ĸ���-��Ϊֻ�Ǹı��Ӧ���ӵ���Ʒ��Ϣ

        RectTransform t = transform as RectTransform;                                     //�ص�ԭ���ĸ�����

        t.offsetMax = -Vector2.one * 1;                   //RectTransform���ϽǾ���ê��ľ���
        t.offsetMin = -Vector2.one * 1;
    }

    public void SwapItem()      //������Ʒ
    {
        //�趨Ŀ������Ҫ��ק����Ʒ
        var targetItem = targetHolder.itemUI.Bag.items[targetHolder.itemUI.Index];        //�ı����˳�򣬴ﵽ��ק��ƷЧ��
        var tempItem = currentHolder.itemUI.Bag.items[currentHolder.itemUI.Index];        //��ǰ��ק����Ʒ

        bool isSameItem = tempItem.itemData == targetItem.itemData;                       //�ж���ק��Ʒ��Ŀ����ӵ���Ʒ�Ƿ���ͬ

        if (isSameItem && targetItem.itemData.stackable)                                  //ͬʱ������ͬ��Ʒ�ҿ��Ե���
        {
            targetItem.amount += tempItem.amount;                                         //����������
            targetItem.itemData = tempItem.itemData;
            tempItem.amount = 0;                                                          //���������Ӻ������ק����

        }
        else
        {
            currentHolder.itemUI.Bag.items[currentHolder.itemUI.Index] = targetItem;       //����ֱ��Ŀ������ק����Ʒ�໥����
            targetHolder.itemUI.Bag.items[targetHolder.itemUI.Index] = tempItem;
        }
    }
}
