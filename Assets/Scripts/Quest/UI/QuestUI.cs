using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : Singleton<QuestUI>
{
    [Header("Elements-�������")]

    public GameObject questPanel;
    public ItemTooTip tooTip;
    bool isOpen;

    [Header("Quest Name-��������")]
    public RectTransform questListTransform;    //��Ҫ���ɵ�λ��-Rectλ��
    public QuestNameButton questNameButton;


    [Header("Text Content-��������")]
    public Text questContentText;

    [Header("Requirement-�����б�")]
    public RectTransform requireTransform;
    public QuestRequirement requirement;

    [Header("Reward Panel-ս��Ʒ")]
    public RectTransform rewardTransform;
    public ItemUI rewardUI;



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isOpen = !isOpen;
            questPanel.SetActive(isOpen);
            questContentText.text = string.Empty;       //�մ��������ʱ���Ҳ�����������ʱΪ��
            //��ʾ�������
            SetupQuestList();

            if (!isOpen)                                //û�д���������ʱ��ر���ʾTooltip
                tooTip.gameObject.SetActive(false);
        }
    }

    public void SetupQuestList()
    {
        foreach (Transform item in questListTransform)
        {
            Destroy(item.gameObject);                  //���ٲ����õ����-Ϊ��

        }
        foreach (Transform item in rewardTransform)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in requireTransform)
        {
            Destroy(item.gameObject);
        }
        //��QuestManager�ҵ����������ɳ�����ť-����λ��
        foreach (var task in QuestManager.Instance.tasks)   
        {
            var newTask = Instantiate(questNameButton,questListTransform);
            newTask.SetupNameButton(task.questData);
            //newTask.questContentText = questContentText;        //���õ�����Ϣ��������ť
        }
    }

    public void SetupRequireList(QuestData_SO questData)
    {
        questContentText.text = questData.description;          //�õ��������ݽ����鴫���Ҳ���ʾ
        foreach (Transform item in requireTransform)           //���ٲ����õ����-Ϊ�� 
        {
            Destroy(item.gameObject);
        }

        foreach (var require in questData.questRequires)        //ѭ����������������
        {
            var q = Instantiate(requirement,requireTransform);  //�����ɳ����������Ԥ֧��-�����ݴ���Ԥ֧��������������
            if (questData.isFinished)                           //�ж������Ƿ����
            {
                q.SetupRequirement(require.name,true);
            }
            else
            {
                q.SetupRequirement(require.name, require.requireAmount, require.currentAmount);
            }
           
        }
    }

    public void SetupRewardItem( ItemData_SO itemData, int amount )
    {
        var item = Instantiate(rewardUI, rewardTransform);
        item.SetUpItemUI(itemData, amount);
    }
}
