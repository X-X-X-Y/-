using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : Singleton<QuestUI>
{
    [Header("Elements-基本组件")]

    public GameObject questPanel;
    public ItemTooTip tooTip;
    bool isOpen;

    [Header("Quest Name-任务名称")]
    public RectTransform questListTransform;    //需要生成的位置-Rect位置
    public QuestNameButton questNameButton;


    [Header("Text Content-任务内容")]
    public Text questContentText;

    [Header("Requirement-需求列表")]
    public RectTransform requireTransform;
    public QuestRequirement requirement;

    [Header("Reward Panel-战利品")]
    public RectTransform rewardTransform;
    public ItemUI rewardUI;



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isOpen = !isOpen;
            questPanel.SetActive(isOpen);
            questContentText.text = string.Empty;       //刚打开任务面板时，右侧任务详情暂时为空
            //显示面板内容
            SetupQuestList();

            if (!isOpen)                                //没有打开任务面板的时候关闭显示Tooltip
                tooTip.gameObject.SetActive(false);
        }
    }

    public void SetupQuestList()
    {
        foreach (Transform item in questListTransform)
        {
            Destroy(item.gameObject);                  //销毁测试用的组件-为空

        }
        foreach (Transform item in rewardTransform)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in requireTransform)
        {
            Destroy(item.gameObject);
        }
        //在QuestManager找到任务，先生成出任务按钮-及其位置
        foreach (var task in QuestManager.Instance.tasks)   
        {
            var newTask = Instantiate(questNameButton,questListTransform);
            newTask.SetupNameButton(task.questData);
            //newTask.questContentText = questContentText;        //把拿到的信息传给任务按钮
        }
    }

    public void SetupRequireList(QuestData_SO questData)
    {
        questContentText.text = questData.description;          //拿到任务数据将详情传给右侧显示
        foreach (Transform item in requireTransform)           //销毁测试用的组件-为空 
        {
            Destroy(item.gameObject);
        }

        foreach (var require in questData.questRequires)        //循环传进的任务需求
        {
            var q = Instantiate(requirement,requireTransform);  //先生成出任务需求的预支体-将数据传给预支体让其设置数据
            if (questData.isFinished)                           //判断任务是否结束
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
