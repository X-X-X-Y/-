using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public enum SlotType            //���ò�ͬ����ĵ�Ԫ��-����-����-����-�����/ս����
{
    BAG,
    WEAPON,
    ARMOR,
    ACTION
}
public class SlotHolder : MonoBehaviour , IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler //unity���������ʱ��
{
    public SlotType slotType;

    public ItemUI itemUI;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount % 2 == 0)                      //˫��������
        {
            UseItem();
        }
    }

    public void UseItem()
    {
        if (itemUI.GetItem() != null)
        {
            if (itemUI.GetItem().itemType == ItemType.Useable && itemUI.Bag.items[itemUI.Index].amount > 0)     //ʹ��ʱ�ж��Ƿ�Ϊ��Ӧ���ͺ�����
            {
                GameManager.Instance.playerStats.ApplyHealth(itemUI.GetItem().useableData.healthPoint);       //ͨ��GameManager�õ�����Ķ���

                itemUI.Bag.items[itemUI.Index].amount -= 1;                                                 //ʹ�ú�������һ������UI

                //ʹ����Ʒ��Ӱ���������
                QuestManager.Instance.UpdateQuestProgress(itemUI.GetItem().itemName,-1);
            }
        }

       
        UpdateItem();
    }

    public void UpdateItem()
    {
        switch (slotType)
        {
            case SlotType.BAG:
            itemUI.Bag = InventoryManager.Instance.inventoryData;         //�ж���Bag�󣬴����ݿ⴫�뱳������

                break;

            case SlotType.WEAPON:
                itemUI.Bag = InventoryManager.Instance.equipmentData;
                // װ���������л����ɣ�����
                if (itemUI.Bag.items[itemUI.Index].itemData != null)    //ȷ�����������
                {
                    GameManager.Instance.playerStats.ChangeWeapon(itemUI.Bag.items[itemUI.Index].itemData);
                }
                else
                {
                    GameManager.Instance.playerStats.UnEquipWeapon();   //�����������;ö��趨���޷�װ��
                }
                break;

            case SlotType.ARMOR:
                itemUI.Bag = InventoryManager.Instance.equipmentData;
                break;

            case SlotType.ACTION:
                itemUI.Bag = InventoryManager.Instance.actionData;
                break;
        }
        var item = itemUI.Bag.items[itemUI.Index];                  //�ҵ�����ı���������items�б���ͬ�����͵���Ʒ
        itemUI.SetUpItemUI(item.itemData,item.amount);              //����Ʒ�������������õ�ItemUI��
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemUI.GetItem())       //�ж�UI���Ƿ�������
        {
            InventoryManager.Instance.toolTip.SetupTooltip(itemUI.GetItem());   //���õ�����Ʒ��Ϣ����������
            InventoryManager.Instance.toolTip.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryManager.Instance.toolTip.gameObject.SetActive(false);
    }

    private void OnDisable()    //��ͣ��ʱ���ֹ����UI�޷��ر�
    {
        InventoryManager.Instance.toolTip.gameObject.SetActive(false);
    }
}
