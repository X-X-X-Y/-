using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    public Text optionText;

    private Button thisButton;

    public DialoguePiece curentPiece;   //定义一个当前对话内容

    private string nextPieceId;

    private bool takeQuest;

    private void Awake()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(OnOptionClicked);                    //挂在按键点击
    }

    public void UpadateOption(DialoguePiece piece , DialogueOption option)  //传入对话内容及选项
    {
        curentPiece = piece;
        optionText.text = option.text;
        nextPieceId = option.targetID;                                      //每个选项拿到自身的目标ID
        takeQuest = option.takeQuest;
    }

    public void OnOptionClicked()
    {
        
        //任务点击逻辑
        if (curentPiece.quest != null)  
        {
            var newTask = new QuestManager.QuestTask        //QuestTask中传入的数据类型与Add需要的不一致
            {
                questData = Instantiate(curentPiece.quest)
            };
            if (takeQuest)
            {
                //添加到任务列表
                //判断是否有任务
                if (QuestManager.Instance.HaveQuest(newTask.questData))
                {
                    //判断任务状态-提交任务判断是否完成
                    if (QuestManager.Instance.GetTask(newTask.questData).IsComplete)    
                    {
                        newTask.questData.GiveReward();
                        QuestManager.Instance.GetTask(newTask.questData).IsFinished = true;
                    }
                }
                else //没有任务新添加传入的任务
                {
                    QuestManager.Instance.tasks.Add(newTask);
                    QuestManager.Instance.GetTask(newTask.questData).IsStarted = true;  //将任务的IsStarted标记为true

                    //接受任务后遍历背包、快捷栏物品，查看物品是否满足任务需求
                    foreach (var requireItem in newTask.questData.RequireTargetName())  //循环当前新任务里需要的物品
                    {
                        InventoryManager.Instance.CheckQuestItemInBag(requireItem);
                    }
                }

            }
        }

        //任务显示逻辑
        if (nextPieceId == "")                                              //如果选项没有跳转则关闭对话
        {
            DialogueUI.Instance.dialoguePanel.SetActive(false);
            return;
        }
        else
        {
            DialogueUI.Instance.UpdateMainDialogue(DialogueUI.Instance.currentData.dialogueIndex[nextPieceId]);
        }
    }
}
