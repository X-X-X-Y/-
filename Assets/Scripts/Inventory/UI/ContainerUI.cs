using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerUI : MonoBehaviour
{
    public SlotHolder[] slotHolders;                 //将“背包的30个格子”设定一个数组

    public void RefreshUI()                         // 将背包物品生成出来
    {
        for (int i = 0; i < slotHolders.Length; i++) //循环所有格子
        {
            slotHolders[i].itemUI.Index = i;         //告诉所有格子自己的编号
            slotHolders[i].UpdateItem();             //更新物品
        }
    }

}
