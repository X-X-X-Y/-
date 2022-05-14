using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : Singleton<DialogueUI>
{
    [Header("Basic Elements-����Ԫ��")]
    public Image icon;
    public Text mainText;
    public Button nextButton;

    public GameObject dialoguePanel;
    

    [Header("Options-ѡ��")]
    public RectTransform optionPanel;

    public OptionUI optionPrefab;

    [Header("Data-����")]
    public DialogueData_SO currentData;
    int currentIndex = 0;                       //��ǰ�Ի��е���һ��-ָ��

    protected override void Awake()             //����ģʽ�£���Ҫ����Awake����
    {
        base.Awake();
        nextButton.onClick.AddListener(ContinueDialogue);
    }

    void ContinueDialogue()                     //��һ���Ի�
    {
        if (currentIndex < currentData.dialoguePieces.Count)
            UpdateMainDialogue(currentData.dialoguePieces[currentIndex]);
        else
            dialoguePanel.SetActive(false);
    }

    public void UpdateDialogueData(DialogueData_SO data)    //��������
    {
        currentData = data;
        currentIndex = 0;
    }

    public void UpdateMainDialogue(DialoguePiece piece)                        //���¶Ի���-��Ҫ������е�ǰ�����е�piece
    {
        dialoguePanel.SetActive(true);

        currentIndex++;

        if (piece.image != null)
        {
            icon.enabled = true;
            icon.sprite = piece.image;
        }
        else
            icon.enabled = false;

        mainText.text = "";         //��ʱ��ղ����ı�
        mainText.text = piece.text;
        

        if (piece.options.Count == 0 && currentData.dialoguePieces.Count > 0)         //�ж��Ƿ�ѡ��Ի�������һ���Ի�
        {
            nextButton.interactable = true;
            nextButton.gameObject.SetActive(true);
            nextButton.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        //nextButton.gameObject.SetActive(false);
        {//�ֶν���������Nextʱ��С��С
            nextButton.interactable = false;                                    //��ֹ�㰴��ť
            nextButton.transform.GetChild(0).gameObject.SetActive(false);
        }

        //����ѡ�� Options
        CreatOptions(piece);
    }

    void CreatOptions(DialoguePiece piece)
    {
        if (optionPanel.childCount > 0)         //ѡ���д���������-����������������
        {
            for (int i = 0; i < optionPanel.childCount; i++)
            {
                Destroy(optionPanel.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < piece.options.Count; i++)       //���ݶԻ����ɶ�Ӧ������ѡ��
        {
            var option = Instantiate(optionPrefab,optionPanel); //���ɳ�Ԥ������optionPanel����
            option.UpadateOption(piece, piece.options[i]);
        }
    }

}
