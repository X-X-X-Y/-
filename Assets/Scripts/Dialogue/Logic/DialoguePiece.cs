using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialoguePiece 
{
    public string ID;                   //对话的序号

    public Sprite image;                //对话时的头像显示

    [TextArea]                          //更大的显示范围
    public string text;                 //文本内容
    public QuestData_SO quest;

    public List<DialogueOption> options = new List<DialogueOption>();           //选择部分的列表

    [HideInInspector]
    public bool canExpand;


}
