using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Dailogue" , menuName =  "Dialogue/Dialogue Data")]
public class DialogueData_SO : ScriptableObject
{
    public List<DialoguePiece> dialoguePieces = new List<DialoguePiece>();

    //使用字典精确查找-对话选项可以跳转至对应对话piece
    public Dictionary<string, DialoguePiece> dialogueIndex = new Dictionary<string, DialoguePiece>();




#if UNITY_EDITOR
    private void OnValidate()   //当前数据在unity被修改时，调用方法
    {
        dialogueIndex.Clear();  //先清空字典
        foreach (var piece in dialoguePieces)
        {
            if (!dialogueIndex.ContainsKey(piece.ID))       //如果没有对应的Key-所以把当前的加入
                dialogueIndex.Add(piece.ID,piece);
        }
    }
#else
    private void Awake()
    {
        dialogueIndex.Clear();
        foreach (var piece in dialoguePieces)
        {
            if (!dialogueIndex.ContainsKey(piece.ID))       //如果没有对应的Key-所以把当前的加入
                dialogueIndex.Add(piece.ID, piece);
        }
    }
#endif

    public QuestData_SO GetQuest()
    {
        QuestData_SO currentData = null;
        foreach (var piece in dialoguePieces)   //遍历所有对话查找有没有任务
        {
            if (piece.quest != null)
                currentData = piece.quest;
        }
        return currentData;
    }
}
