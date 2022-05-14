using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public Image icon = null;
    public Text amount = null;
    public ItemData_SO currentItemData;

    public InventoryData_SO Bag { get; set;}   //设置背包属性-对应什么样的数据库

    public int Index { get; set; } = -1;        //初始值-1，Index序号从0开始

    public void SetUpItemUI(ItemData_SO item, int itemAmount)
    {
        if (itemAmount == 0)
        {
            Bag.items[Index].itemData = null;
            icon.gameObject.SetActive(false);               //数量为0直接关闭图片显示
            return;
        }

        if (itemAmount < 0)                                 //特殊情况-提交任务时，扣除物品错误显示在奖励物品上
        {
            item = null;
        }

        if (item != null)                                   //拿到数据库传入的item并更新UI-显示出来
        {
            currentItemData = item;                         //将拿到的数据给到当前物品数据-任务战利品
            icon.sprite = item.itemIcon;
            amount.text = itemAmount.ToString();
            icon.gameObject.SetActive(true);
        }
        else
        {
            icon.gameObject.SetActive(false);
        }
    }

    public ItemData_SO GetItem()                            //返回物品背包中对应的Index，方便后续代码书写
    {
        return Bag.items[Index].itemData;
    }

}
