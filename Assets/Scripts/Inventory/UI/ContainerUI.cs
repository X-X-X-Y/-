using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerUI : MonoBehaviour
{
    public SlotHolder[] slotHolders;                 //����������30�����ӡ��趨һ������

    public void RefreshUI()                         // ��������Ʒ���ɳ���
    {
        for (int i = 0; i < slotHolders.Length; i++) //ѭ�����и���
        {
            slotHolders[i].itemUI.Index = i;         //�������и����Լ��ı��
            slotHolders[i].UpdateItem();             //������Ʒ
        }
    }

}
