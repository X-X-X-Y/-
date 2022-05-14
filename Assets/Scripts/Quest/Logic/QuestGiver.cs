using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueController))]  //��ֹ�ű�������-ʹ�ô˽ű�ʱ������ȷ����ǰ�����DialogueController
public class QuestGiver : MonoBehaviour
{
    DialogueController controller;
    QuestData_SO currentQuest;

    public DialogueData_SO startDialogue;
    public DialogueData_SO progressDialogue;
    public DialogueData_SO completeDialogue;
    public DialogueData_SO finishDialogue;

    #region �������ǰ״̬
    public bool IsStarted
    {
        get //�Ƿ��������Ѿ���ʼ
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
        get //�Ƿ��������Ѿ���ʼ
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
        get //�Ƿ��������Ѿ���ʼ
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
        //��Ϸһ��ʼΪ��ʼ����Ի�-���öԻ������е�����
        controller.currentData = startDialogue;
        currentQuest = controller.currentData.GetQuest();
    }

    private void Update()
    {
        if (IsStarted)  //�����Ѿ���ʼ
        {
            if (IsComplete) //���������
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
