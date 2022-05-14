using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest Data")]
public class QuestData_SO : ScriptableObject
{
    [System.Serializable]
    //����Ҫ��-����-��������-��ǰ����
    public class QuestRequire
    {
        public string name;
        public int requireAmount;
        public int currentAmount;
    }

    public string questName;

    [TextArea]
    public string description;

    public bool isStarted;          //���������״̬
    public bool isComplete;
    public bool isFinished;         //�Ѿ��ύ


    public List<QuestRequire> questRequires = new List<QuestRequire>();

    public List<InventoryItem> rewards = new List<InventoryItem>();

    //����������
    public void CheckQuestProgress()
    {
        //where-ѡ������������-�Ƚϵ�ǰ����������������Ƿ����/����
        var finishRequires = questRequires.Where(r => r.requireAmount <= r.currentAmount);
        //��дboolֵ����-��������true
        isComplete = finishRequires.Count() == questRequires.Count;

        if (isComplete) 
        {
            Debug.Log("����");
        }
    }


    //��ǰ������Ʒ����  �ռ�/����  ��Ŀ�������б�
    public List<string> RequireTargetName()
    {
        List<string> targetNameList = new List<string>();       //����һ������Ŀ�������б�

        foreach (var requier in questRequires)                  //�������������б�����������Ŀ�������б�
        {
            targetNameList.Add(requier.name);
        }


        return targetNameList;
    }

    //������
    public void GiveReward()
    {
        foreach (var reward in rewards)    //�����б������н���
        {   
            //��Ҫ�۳��ĵ���-����ֵ�������������߼�
            if (reward.amount < 0)
            {
                int requireCount = Mathf.Abs(reward.amount);

                //����-�����-����Ϳ������������
                if (InventoryManager.Instance.QuestItemInBag(reward.itemData) != null)  
                {
                    //������������/�㹻
                    if (InventoryManager.Instance.QuestItemInBag(reward.itemData).amount <= requireCount)    
                    {
                        requireCount -= InventoryManager.Instance.QuestItemInBag(reward.itemData).amount;
                        InventoryManager.Instance.QuestItemInBag(reward.itemData).amount = 0;

                        if (InventoryManager.Instance.QuestItemInAction(reward.itemData) != null)   //�������������Ļ��ӿ�����۳�
                            InventoryManager.Instance.QuestItemInAction(reward.itemData).amount -= requireCount;
                    }
                    else
                    {
                        InventoryManager.Instance.QuestItemInBag(reward.itemData).amount -= requireCount;
                    }
                }
                else //����û����Ҫ�۳�����Ʒ-�ڿ����ֱ�ӿ۳�
                {
                    InventoryManager.Instance.QuestItemInAction(reward.itemData).amount -= requireCount;
                }

            }
            else
            {   //��������-ֱ����ӵ���Ʒ��
                InventoryManager.Instance.inventoryData.AddItem(reward.itemData,reward.amount);
            }
            //������Ӻ����UI
            InventoryManager.Instance.inventoryUI.RefreshUI();
            InventoryManager.Instance.actionUI.RefreshUI();
        }
    }
}
