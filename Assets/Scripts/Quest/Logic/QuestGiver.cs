using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueController))]  //防止脚本被乱用-使用此脚本时，必须确保当前组件有DialogueController
public class QuestGiver : MonoBehaviour
{
    DialogueController controller;
    QuestData_SO currentQuest;

    public DialogueData_SO startDialogue;
    public DialogueData_SO progressDialogue;
    public DialogueData_SO completeDialogue;
    public DialogueData_SO finishDialogue;

    #region 获得任务当前状态
    public bool IsStarted
    {
        get //是否有任务已经开始
        {
            if (QuestManager.Instance.HaveQuest(currentQuest))
            {
                return QuestManager.Instance.GetTask(currentQuest).IsStarted;
            }
            else
                return false;
        }
    }
    public bool IsComplete
    {
        get //是否有任务已经开始
        {
            if (QuestManager.Instance.HaveQuest(currentQuest))
            {
                return QuestManager.Instance.GetTask(currentQuest).IsComplete;
            }
            else
                return false;
        }
    }
    public bool IsFinished
    {
        get //是否有任务已经开始
        {
            if (QuestManager.Instance.HaveQuest(currentQuest))
            {
                return QuestManager.Instance.GetTask(currentQuest).IsFinished;
            }
            else
                return false;
        }
    }
    #endregion

    private void Awake()
    {
        controller = GetComponent<DialogueController>();
    }

    private void Start()
    {
        //游戏一开始为开始任务对话-并拿对话数据中的任务
        controller.currentData = startDialogue;
        currentQuest = controller.currentData.GetQuest();
    }

    private void Update()
    {
        if (IsStarted)  //任务已经开始
        {
            if (IsComplete) //任务已完成
            {
                controller.currentData = completeDialogue;
            }
            else
            {
                controller.currentData = progressDialogue;
            }
        }

        if (IsFinished)
        {
            controller.currentData = finishDialogue;
        }
    }
}
