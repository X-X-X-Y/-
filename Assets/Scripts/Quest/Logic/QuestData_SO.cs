using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest Data")]
public class QuestData_SO : ScriptableObject
{
    [System.Serializable]
    //任务要求-名称-需求数量-当前数量
    public class QuestRequire
    {
        public string name;
        public int requireAmount;
        public int currentAmount;
    }

    public string questName;

    [TextArea]
    public string description;

    public bool isStarted;          //任务的三种状态
    public bool isComplete;
    public bool isFinished;         //已经提交


    public List<QuestRequire> questRequires = new List<QuestRequire>();

    public List<InventoryItem> rewards = new List<InventoryItem>();

    //检查任务进度
    public void CheckQuestProgress()
    {
        //where-选择条件并返回-比较当前任务与需求的数量是否大于/等于
        var finishRequires = questRequires.Where(r => r.requireAmount <= r.currentAmount);
        //简写bool值变量-如果满足就true
        isComplete = finishRequires.Count() == questRequires.Count;

        if (isComplete) 
        {
            Debug.Log("拿下");
        }
    }


    //当前任务物品需求  收集/消灭  的目标名字列表
    public List<string> RequireTargetName()
    {
        List<string> targetNameList = new List<string>();       //创建一个需求目标名字列表

        foreach (var requier in questRequires)                  //遍历任务需求列表并给到创建的目标名字列表
        {
            targetNameList.Add(requier.name);
        }


        return targetNameList;
    }

    //任务奖励
    public void GiveReward()
    {
        foreach (var reward in rewards)    //遍历列表中所有奖励
        {   
            //需要扣除的道具-绝对值化需求量方便逻辑
            if (reward.amount < 0)
            {
                int requireCount = Mathf.Abs(reward.amount);

                //包里-快捷栏-包里和快捷栏的总数量
                if (InventoryManager.Instance.QuestItemInBag(reward.itemData) != null)  
                {
                    //包里数量不够/足够
                    if (InventoryManager.Instance.QuestItemInBag(reward.itemData).amount <= requireCount)    
                    {
                        requireCount -= InventoryManager.Instance.QuestItemInBag(reward.itemData).amount;
                        InventoryManager.Instance.QuestItemInBag(reward.itemData).amount = 0;

                        if (InventoryManager.Instance.QuestItemInAction(reward.itemData) != null)   //包里数量不够的话从快捷栏扣除
                            InventoryManager.Instance.QuestItemInAction(reward.itemData).amount -= requireCount;
                    }
                    else
                    {
                        InventoryManager.Instance.QuestItemInBag(reward.itemData).amount -= requireCount;
                    }
                }
                else //包里没有需要扣除的物品-在快捷栏直接扣除
                {
                    InventoryManager.Instance.QuestItemInAction(reward.itemData).amount -= requireCount;
                }

            }
            else
            {   //正常奖励-直接添加到物品中
                InventoryManager.Instance.inventoryData.AddItem(reward.itemData,reward.amount);
            }
            //数据添加后更新UI
            InventoryManager.Instance.inventoryUI.RefreshUI();
            InventoryManager.Instance.actionUI.RefreshUI();
        }
    }
}
