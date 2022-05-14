using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialoguePiece 
{
    public string ID;                   //�Ի������

    public Sprite image;                //�Ի�ʱ��ͷ����ʾ

    [TextArea]                          //�������ʾ��Χ
    public string text;                 //�ı�����
    public QuestData_SO quest;

    public List<DialogueOption> options = new List<DialogueOption>();           //ѡ�񲿷ֵ��б�

    [HideInInspector]
    public bool canExpand;


}
