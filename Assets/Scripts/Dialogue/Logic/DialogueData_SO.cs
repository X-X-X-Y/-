using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Dailogue" , menuName =  "Dialogue/Dialogue Data")]
public class DialogueData_SO : ScriptableObject
{
    public List<DialoguePiece> dialoguePieces = new List<DialoguePiece>();

    //ʹ���ֵ侫ȷ����-�Ի�ѡ�������ת����Ӧ�Ի�piece
    public Dictionary<string, DialoguePiece> dialogueIndex = new Dictionary<string, DialoguePiece>();




#if UNITY_EDITOR
    private void OnValidate()   //��ǰ������unity���޸�ʱ�����÷���
    {
        dialogueIndex.Clear();  //������ֵ�
        foreach (var piece in dialoguePieces)
        {
            if (!dialogueIndex.ContainsKey(piece.ID))       //���û�ж�Ӧ��Key-���԰ѵ�ǰ�ļ���
                dialogueIndex.Add(piece.ID,piece);
        }
    }
#else
    private void Awake()
    {
        dialogueIndex.Clear();
        foreach (var piece in dialoguePieces)
        {
            if (!dialogueIndex.ContainsKey(piece.ID))       //���û�ж�Ӧ��Key-���԰ѵ�ǰ�ļ���
                dialogueIndex.Add(piece.ID, piece);
        }
    }
#endif

    public QuestData_SO GetQuest()
    {
        QuestData_SO currentData = null;
        foreach (var piece in dialoguePieces)   //�������жԻ�������û������
        {
            if (piece.quest != null)
                currentData = piece.quest;
        }
        return currentData;
    }
}
