using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Inventory Data")]
public class InventoryData_SO : ScriptableObject
{
    //��InventoryData_SO������Ʒ�б�
    public List<InventoryItem> items = new List<InventoryItem>();

    public void AddItem( ItemData_SO newItemData,int amount )
    {
        bool found = false;

        if (newItemData.stackable)
        {
            foreach (var item in items)                      //�����б���������Ʒ
            {
                if (item.itemData == newItemData)
                {   
                    item.amount += amount;                  //����¼ӽ�������Ʒ��֮ǰ�������Ƶģ�ֱ���������-���ҵ�-�߳�ѭ��
                    found = true;
                    break;
                }
            }
        }
      
        for (int i = 0; i < items.Count; i++)                 //��������ǿյ�-û��ͬ��-�ҵ�����Ŀո񡢽���Ʒ����
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

//���л��б��ܹ��ڱ༭���п���public����
[System.Serializable]         
//�������-�����Ʒ������
public class InventoryItem
{
    
    public ItemData_SO itemData;

    public int amount;

}
