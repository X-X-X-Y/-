using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public Image icon = null;
    public Text amount = null;
    public ItemData_SO currentItemData;

    public InventoryData_SO Bag { get; set;}   //���ñ�������-��Ӧʲô�������ݿ�

    public int Index { get; set; } = -1;        //��ʼֵ-1��Index��Ŵ�0��ʼ

    public void SetUpItemUI(ItemData_SO item, int itemAmount)
    {
        if (itemAmount == 0)
        {
            Bag.items[Index].itemData = null;
            icon.gameObject.SetActive(false);               //����Ϊ0ֱ�ӹر�ͼƬ��ʾ
            return;
        }

        if (itemAmount < 0)                                 //�������-�ύ����ʱ���۳���Ʒ������ʾ�ڽ�����Ʒ��
        {
            item = null;
        }

        if (item != null)                                   //�õ����ݿ⴫���item������UI-��ʾ����
        {
            currentItemData = item;                         //���õ������ݸ�����ǰ��Ʒ����-����ս��Ʒ
            icon.sprite = item.itemIcon;
            amount.text = itemAmount.ToString();
            icon.gameObject.SetActive(true);
        }
        else
        {
            icon.gameObject.SetActive(false);
        }
    }

    public ItemData_SO GetItem()                            //������Ʒ�����ж�Ӧ��Index���������������д
    {
        return Bag.items[Index].itemData;
    }

}
