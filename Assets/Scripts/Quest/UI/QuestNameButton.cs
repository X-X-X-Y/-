using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QuestNameButton : MonoBehaviour
{
    public Text questNameText;

    public QuestData_SO currentDara;        //��������

    //public Text questContentText;           //������Ϣ˵��

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(UpdtaeQuestContent);
    }

    //�������ťʱ��Ҫ������¼�     
    void   UpdtaeQuestContent()
    {
        //questContentText.text = currentDara.description;
        QuestUI.Instance.SetupRequireList(currentDara);

        //��ʾ������Ʒǰ����֮ǰչʾ���Ľ���
        foreach (Transform item in QuestUI.Instance.rewardTransform)
        {
            Destroy(item.gameObject);
        }

        //ѭ�����н���-����QuestUI����UI
        foreach (var item in currentDara.rewards)
        {
            QuestUI.Instance.SetupRewardItem(item.itemData,item.amount);
        }
        
    }

    public void SetupNameButton(QuestData_SO questData)
    {
        currentDara = questData;

        if (questData.isComplete)
            questNameText.text = questData.questName + "(���)";
        else
            questNameText.text = questData.questName;
    }


}
