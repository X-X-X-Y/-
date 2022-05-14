using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemData_SO itemData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //TODO:加入背包
            InventoryManager.Instance.inventoryData.AddItem(itemData,itemData.itemAmount);      //向数据库添加捡到的物品
            InventoryManager.Instance.inventoryUI.RefreshUI();                                  //向UI更新捡到的物品信息
            //装备武器
            //GameManager.Instance.playerStats.EquipWeapon(itemData);             //通过GameManager访问到玩家数据

            //检查是否有任务
            QuestManager.Instance.UpdateQuestProgress(itemData.itemName,itemData.itemAmount);

            Destroy(gameObject);
        }
    }
}
