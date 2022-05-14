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
            //TODO:���뱳��
            InventoryManager.Instance.inventoryData.AddItem(itemData,itemData.itemAmount);      //�����ݿ���Ӽ񵽵���Ʒ
            InventoryManager.Instance.inventoryUI.RefreshUI();                                  //��UI���¼񵽵���Ʒ��Ϣ
            //װ������
            //GameManager.Instance.playerStats.EquipWeapon(itemData);             //ͨ��GameManager���ʵ��������

            //����Ƿ�������
            QuestManager.Instance.UpdateQuestProgress(itemData.itemName,itemData.itemAmount);

            Destroy(gameObject);
        }
    }
}
