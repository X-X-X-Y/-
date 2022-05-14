using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;    //�ص�
using UnityEditorInternal;
using System;
using System.IO;

[CustomEditor(typeof(DialogueData_SO))]             //ֻ��Ӱ��DialogueData_SO
public class DialogueCustomEditor : Editor          //�򿪶�Ӧ�ĶԻ�����
{
    public override void OnInspectorGUI()           //��Inspector����
    {
        if (GUILayout.Button("Open in Editor-�򿪲��"))   
        {
            DialogueEditor.InitWindow((DialogueData_SO)target);
        }
        base.OnInspectorGUI();
    }
}

public class DialogueEditor : EditorWindow
{
    DialogueData_SO currentData;

    ReorderableList pieceList = null;

    Vector2 scrollPos = Vector2.zero;           //���ƻ�����



    //��ȡ��ǰ�Ի���ѡ��-�����ֵ�һһƥ��
    Dictionary<string, ReorderableList> optionListDict = new Dictionary<string, ReorderableList>();

    [MenuItem("XXXY/Dialogue Editor")]  //���ڴ���
    public static void Init()
    {
        DialogueEditor editorWindow = GetWindow<DialogueEditor>("Dialogue Editor");      //�����µĴ���
        editorWindow.autoRepaintOnSceneChange = true;                                    //���ڱ仯ʱ���¼���

    }

    public static void InitWindow(DialogueData_SO data)
    {
        DialogueEditor editorWindow = GetWindow<DialogueEditor>("Dialogue Editor");     //���°�ť��򿪴���
        editorWindow.currentData = data;                                                //��Data���ݴ��봰��
    }

    [OnOpenAsset]   //�ص���ʽ-˫���򿪲������
    public static bool OpenAsset(int instancID, int line)                               //instancID-��ǰʵ���������ID
    {
        DialogueData_SO data = EditorUtility.InstanceIDToObject(instancID) as DialogueData_SO;

        if (data != null)
        {
            DialogueEditor.InitWindow(data);
            return true;
        }
        return false;
    }

    //ѡ��ı�ʱ����һ��
    private void OnSelectionChange()
    {
        var newData = Selection.activeObject as DialogueData_SO;
        if (newData != null)
        {
            currentData = newData;
            SetupReorderableList();
        }
        else
        {
            currentData = null;
            pieceList = null;
        }
        Repaint();  //���»���һ�Σ�ǿ��ִ��OnGUI
    }


    //������ʾ����
    private void OnGUI()
    {
        if (currentData != null)
        {
            EditorGUILayout.LabelField(currentData.name, EditorStyles.boldLabel);    //������ǰ��������-������ʾ
            GUILayout.Space(10);                                                    //���

            //���ƻ�����-���Ż������Ļ�����ʱ�������������ְ
            scrollPos = GUILayout.BeginScrollView(scrollPos,GUILayout.ExpandWidth(true),GUILayout.ExpandHeight(true));

            if (pieceList == null)                                                  //�пպ�滭�µ��б�
                SetupReorderableList();
            pieceList.DoLayoutList();
            GUILayout.EndScrollView();
        }
        else
        {
            if (GUILayout.Button("Create New Dialogue-����һ���µĶԻ�"))   
            {
                string dataPath = "Assets/GameData/Dialogue Data/";
                if (!Directory.Exists(dataPath))
                {
                    Directory.CreateDirectory(dataPath);
                }
                DialogueData_SO newData = ScriptableObject.CreateInstance<DialogueData_SO>();   //����һ������
                AssetDatabase.CreateAsset(newData,dataPath + "/" + "New Dialogue.asset");       //�����ļ�����·�������ݴ���
                currentData = newData;
            }
            GUILayout.Label("No Data Selected!", EditorStyles.boldLabel);
        }
    }

    private void OnDisable()    //�ֵ�����������ʱ����
    {
        optionListDict.Clear();
    }

    private void SetupReorderableList()
    {
        //�����б�-����4��true��ʾ�Ƿ�ɱ༭
        pieceList = new ReorderableList(currentData.dialoguePieces,typeof(DialoguePiece),true,true,true,true);

        //��������Ԫ��
        pieceList.drawHeaderCallback += OnDrawPieceHeader;
        pieceList.drawElementCallback += OnDrawPieceListElement;
        pieceList.elementHeightCallback += OnHeightChanged;             //�߶ȸ���
    }

    private float OnHeightChanged(int index)
    {
        return GetPieceHeight(currentData.dialoguePieces[index]);
    }

    //��ȡ��Ҫ���ӵĸ߶�
    float GetPieceHeight(DialoguePiece piece)
    {
        var height = EditorGUIUtility.singleLineHeight;

        var isExpand = piece.canExpand;
        if (isExpand)
        {

            height += EditorGUIUtility.singleLineHeight * 9;

            var options = piece.options;    //ѡ��߶ȱ仯

            if (options.Count > 1)
            {
                height += EditorGUIUtility.singleLineHeight * options.Count;
            }
        }
        return height;

    }

    private void OnDrawPieceListElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        //��Ǻ�-ʵʱ�����޸�����
        EditorUtility.SetDirty(currentData);

        GUIStyle textStyle = new GUIStyle("TextField");

        if (index < currentData.dialoguePieces.Count)   //index-���ݱ����index
        {
            var currentPiece = currentData.dialoguePieces[index];

            var tempRect = rect;    //��ʱ��С-ʹ��Ĭ�ϸ߶�

            //���Ʊ�ǩ
            tempRect.height = EditorGUIUtility.singleLineHeight;

            currentPiece.canExpand = EditorGUI.Foldout(tempRect, currentPiece.canExpand, currentPiece.ID);
            if (currentPiece.canExpand)
            {
                tempRect.width = 30;
                tempRect.y += tempRect.height;
                EditorGUI.LabelField(tempRect, "ID");

                //������ʾ�Ի�ID
                tempRect.x += tempRect.width;
                tempRect.width = 100;
                currentPiece.ID = EditorGUI.TextField(tempRect, currentPiece.ID);

                //��������
                tempRect.x += tempRect.width + 10;
                EditorGUI.LabelField(tempRect, "Quest-����");

                tempRect.x += 75;
                currentPiece.quest = (QuestData_SO)EditorGUI.ObjectField(tempRect, currentPiece.quest, typeof(QuestData_SO), false);

                //����ͼƬ
                tempRect.y += EditorGUIUtility.singleLineHeight + 5;
                tempRect.x = rect.x;
                tempRect.height = 60;
                tempRect.width = tempRect.height;
                currentPiece.image = (Sprite)EditorGUI.ObjectField(tempRect, currentPiece.image, typeof(Sprite), false);

                //�����ı���
                tempRect.x += tempRect.width + 5;
                tempRect.width = rect.width - tempRect.x;
                textStyle.wordWrap = true;
                currentPiece.text = (string)EditorGUI.TextArea(tempRect, currentPiece.text, textStyle);

                //�б��л��б� -ѡ��
                tempRect.y += tempRect.height + 5;
                tempRect.x = rect.x;
                tempRect.width = rect.width;


                //ȷ��Key�Ƿ����ֵ���-���򴴽��µ��б�����ֵ�
                string optionListKey = currentPiece.ID + currentPiece.text; //ָ��������˫��ȷ��

                if (optionListKey != null)
                {
                    if (!optionListDict.ContainsKey(optionListKey))
                    {   //����ֵ���û���ȴ����б�
                        var optionList = new ReorderableList(currentPiece.options, typeof(DialogueOption), true, true, true, true);

                        optionList.drawHeaderCallback = OnDrawOptionHander;

                        optionList.drawElementCallback = (optionRect, optionIndex, optionActive, optionFocused) =>
                         {
                             OnDrawOptionElement(currentPiece,optionRect, optionIndex, optionActive, optionFocused);

                         };

                        //���б��浽�ֵ���
                        optionListDict[optionListKey] = optionList;
                    }
                    optionListDict[optionListKey].DoList(tempRect);
                }
            }
        }
    }

    private void OnDrawOptionHander(Rect rect)
    {
        GUI.Label(rect,"Option Text");
        rect.x += rect.width * 0.5f + 10;
        GUI.Label(rect , "Target ID");
        rect.x += rect.width * 0.3f;
        GUI.Label(rect, "Take Quest");
        
    }

    private void OnDrawOptionElement(DialoguePiece currentPiece, Rect optionRect, int optionIndex, bool optionActive, bool optionFocused)
    {
        var currentOption = currentPiece.options[optionIndex];
        var tempRect = optionRect;

        tempRect.width = optionRect.width * 0.5f;
        currentOption.text = EditorGUI.TextField(tempRect,currentOption.text);

        tempRect.x += tempRect.width + 5;
        tempRect.width = optionRect.width * 0.3f;
        currentOption.targetID = EditorGUI.TextField(tempRect,currentOption.targetID);

        tempRect.x += tempRect.width + 5;
        tempRect.width = optionRect.width * 0.2f;
        currentOption.takeQuest = EditorGUI.Toggle(tempRect,currentOption.takeQuest);
    }

    private void OnDrawPieceHeader(Rect rect)
    {
        GUI.Label(rect,"Dialogue Pieces �Ի�����");
    }
}
