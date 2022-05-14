using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public enum SlotType            //设置不同种类的单元格-背包-武器-盾牌-快捷栏/战斗栏
{
    BAG,
    WEAPON,
    ARMOR,
    ACTION
}
public class SlotHolder : MonoBehaviour , IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler //unity鼠标点击回溯时间
{
    public SlotType slotType;

    public ItemUI itemUI;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount % 2 == 0)                      //双击的整数
        {
            UseItem();
        }
    }

    public void UseItem()
    {
        if (itemUI.GetItem() != null)
        {
            if (itemUI.GetItem().itemType == ItemType.Useable && itemUI.Bag.items[itemUI.Index].amount > 0)     //使用时判断是否为对应类型和数量
            {
                GameManager.Instance.playerStats.ApplyHealth(itemUI.GetItem().useableData.healthPoint);       //通过GameManager拿到点击的东西

                itemUI.Bag.items[itemUI.Index].amount -= 1;                                                 //使用后数量减一、更新UI

                //使用物品会影响任务进度
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
            itemUI.Bag = InventoryManager.Instance.inventoryData;         //判断是Bag后，从数据库传入背包数据

                break;

            case SlotType.WEAPON:
                itemUI.Bag = InventoryManager.Instance.equipmentData;
                // 装备武器、切换（旧）武器
                if (itemUI.Bag.items[itemUI.Index].itemData != null)    //确保不会空引用
                {
                    GameManager.Instance.playerStats.ChangeWeapon(itemUI.Bag.items[itemUI.Index].itemData);
                }
                else
                {
                    GameManager.Instance.playerStats.UnEquipWeapon();   //若果有武器耐久度设定则无法装备
                }
                break;

            case SlotType.ARMOR:
                itemUI.Bag = InventoryManager.Instance.equipmentData;
                break;

            case SlotType.ACTION:
                itemUI.Bag = InventoryManager.Instance.actionData;
                break;
        }
        var item = itemUI.Bag.items[itemUI.Index];                  //找到传入的背包数据中items列表中同样类型的物品
        itemUI.SetUpItemUI(item.itemData,item.amount);              //将物品数据与数量设置到ItemUI中
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemUI.GetItem())       //判断UI内是否有数据
        {
            InventoryManager.Instance.toolTip.SetupTooltip(itemUI.GetItem());   //将拿到的物品信息传给新西兰
            InventoryManager.Instance.toolTip.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryManager.Instance.toolTip.gameObject.SetActive(false);
    }

    private void OnDisable()    //悬停的时候防止其他UI无法关闭
    {
        InventoryManager.Instance.toolTip.gameObject.SetActive(false);
    }
}
