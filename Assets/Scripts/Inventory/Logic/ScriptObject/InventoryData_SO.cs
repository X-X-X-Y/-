using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Inventory Data")]
public class InventoryData_SO : ScriptableObject
{
    //在InventoryData_SO创建物品列表
    public List<InventoryItem> items = new List<InventoryItem>();

    public void AddItem( ItemData_SO newItemData,int amount )
    {
        bool found = false;

        if (newItemData.stackable)
        {
            foreach (var item in items)                      //遍历列表里所有物品
            {
                if (item.itemData == newItemData)
                {   
                    item.amount += amount;                  //如果新加进来的物品与之前的有相似的，直接数量相加-已找到-走出循环
                    found = true;
                    break;
                }
            }
        }
      
        for (int i = 0; i < items.Count; i++)                 //如果背包是空的-没有同类-找到最近的空格、将物品传入
        {
            if (items[i].itemData == null && !found)
            {
                items[i].itemData = newItemData;
                items[i].amount = amount;
                break;
            }
        }
    }
}

//序列化列表，能够在编辑器中看到public数据
[System.Serializable]         
//方便计数-针对物品相加相减
public class InventoryItem
{
    
    public ItemData_SO itemData;

    public int amount;

}
