using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    public Text optionText;

    private Button thisButton;

    public DialoguePiece curentPiece;   //����һ����ǰ�Ի�����

    private string nextPieceId;

    private bool takeQuest;

    private void Awake()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(OnOptionClicked);                    //���ڰ������
    }

    public void UpadateOption(DialoguePiece piece , DialogueOption option)  //����Ի����ݼ�ѡ��
    {
        curentPiece = piece;
        optionText.text = option.text;
        nextPieceId = option.targetID;                                      //ÿ��ѡ���õ������Ŀ��ID
        takeQuest = option.takeQuest;
    }

    public void OnOptionClicked()
    {
        
        //�������߼�
        if (curentPiece.quest != null)  
        {
            var newTask = new QuestManager.QuestTask        //QuestTask�д��������������Add��Ҫ�Ĳ�һ��
            {
                questData = Instantiate(curentPiece.quest)
            };
            if (takeQuest)
            {
                //��ӵ������б�
                //�ж��Ƿ�������
                if (QuestManager.Instance.HaveQuest(newTask.questData))
                {
                    //�ж�����״̬-�ύ�����ж��Ƿ����
                    if (QuestManager.Instance.GetTask(newTask.questData).IsComplete)    
                    {
                        newTask.questData.GiveReward();
                        QuestManager.Instance.GetTask(newTask.questData).IsFinished = true;
                    }
                }
                else //û����������Ӵ��������
                {
                    QuestManager.Instance.tasks.Add(newTask);
                    QuestManager.Instance.GetTask(newTask.questData).IsStarted = true;  //�������IsStarted���Ϊtrue

                    //�������������������������Ʒ���鿴��Ʒ�Ƿ�������������
                    foreach (var requireItem in newTask.questData.RequireTargetName())  //ѭ����ǰ����������Ҫ����Ʒ
                    {
                        InventoryManager.Instance.CheckQuestItemInBag(requireItem);
                    }
                }

            }
        }

        //������ʾ�߼�
        if (nextPieceId == "")                                              //���ѡ��û����ת��رնԻ�
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
