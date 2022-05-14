using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QuestNameButton : MonoBehaviour
{
    public Text questNameText;

    public QuestData_SO currentDara;        //任务数据

    //public Text questContentText;           //任务信息说明

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(UpdtaeQuestContent);
    }

    //当点击按钮时需要处理的事件     
    void   UpdtaeQuestContent()
    {
        //questContentText.text = currentDara.description;
        QuestUI.Instance.SetupRequireList(currentDara);

        //显示奖励物品前销毁之前展示过的奖励
        foreach (Transform item in QuestUI.Instance.rewardTransform)
        {
            Destroy(item.gameObject);
        }

        //循环所有奖励-传给QuestUI设置UI
        foreach (var item in currentDara.rewards)
        {
            QuestUI.Instance.SetupRewardItem(item.itemData,item.amount);
        }
        
    }

    public void SetupNameButton(QuestData_SO questData)
    {
        currentDara = questData;

        if (questData.isComplete)
            questNameText.text = questData.questName + "(完成)";
        else
            questNameText.text = questData.questName;
    }


}
