using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : Singleton<DialogueUI>
{
    [Header("Basic Elements-基本元素")]
    public Image icon;
    public Text mainText;
    public Button nextButton;

    public GameObject dialoguePanel;
    

    [Header("Options-选项")]
    public RectTransform optionPanel;

    public OptionUI optionPrefab;

    [Header("Data-数据")]
    public DialogueData_SO currentData;
    int currentIndex = 0;                       //当前对话中的哪一条-指针

    protected override void Awake()             //单例模式下，需要重载Awake方法
    {
        base.Awake();
        nextButton.onClick.AddListener(ContinueDialogue);
    }

    void ContinueDialogue()                     //下一个对话
    {
        if (currentIndex < currentData.dialoguePieces.Count)
            UpdateMainDialogue(currentData.dialoguePieces[currentIndex]);
        else
            dialoguePanel.SetActive(false);
    }

    public void UpdateDialogueData(DialogueData_SO data)    //更新数据
    {
        currentData = data;
        currentIndex = 0;
    }

    public void UpdateMainDialogue(DialoguePiece piece)                        //更新对话框-需要获得所有当前数据中的piece
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

        mainText.text = "";         //暂时清空测试文本
        mainText.text = piece.text;
        

        if (piece.options.Count == 0 && currentData.dialoguePieces.Count > 0)         //判断是否选项对话且有下一条对话
        {
            nextButton.interactable = true;
            nextButton.gameObject.SetActive(true);
            nextButton.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        //nextButton.gameObject.SetActive(false);
        {//字段较少且内有Next时缩小过小
            nextButton.interactable = false;                                    //禁止点按按钮
            nextButton.transform.GetChild(0).gameObject.SetActive(false);
        }

        //创建选项 Options
        CreatOptions(piece);
    }

    void CreatOptions(DialoguePiece piece)
    {
        if (optionPanel.childCount > 0)         //选项中存在子物体-先销毁所有子物体
        {
            for (int i = 0; i < optionPanel.childCount; i++)
            {
                Destroy(optionPanel.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < piece.options.Count; i++)       //根据对话生成对应数量的选项
        {
            var option = Instantiate(optionPrefab,optionPanel); //生成出预制体在optionPanel下面
            option.UpadateOption(piece, piece.options[i]);
        }
    }

}
