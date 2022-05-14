using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;    //回调
using UnityEditorInternal;
using System;
using System.IO;

[CustomEditor(typeof(DialogueData_SO))]             //只会影响DialogueData_SO
public class DialogueCustomEditor : Editor          //打开对应的对话数据
{
    public override void OnInspectorGUI()           //在Inspector绘制
    {
        if (GUILayout.Button("Open in Editor-打开插件"))   
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

    Vector2 scrollPos = Vector2.zero;           //绘制滑动条



    //获取当前对话的选项-利用字典一一匹配
    Dictionary<string, ReorderableList> optionListDict = new Dictionary<string, ReorderableList>();

    [MenuItem("XXXY/Dialogue Editor")]  //窗口创建
    public static void Init()
    {
        DialogueEditor editorWindow = GetWindow<DialogueEditor>("Dialogue Editor");      //生成新的窗口
        editorWindow.autoRepaintOnSceneChange = true;                                    //窗口变化时重新加载

    }

    public static void InitWindow(DialogueData_SO data)
    {
        DialogueEditor editorWindow = GetWindow<DialogueEditor>("Dialogue Editor");     //按下按钮后打开窗口
        editorWindow.currentData = data;                                                //将Data数据传入窗口
    }

    [OnOpenAsset]   //回调方式-双击打开插件窗口
    public static bool OpenAsset(int instancID, int line)                               //instancID-当前实例化物体的ID
    {
        DialogueData_SO data = EditorUtility.InstanceIDToObject(instancID) as DialogueData_SO;

        if (data != null)
        {
            DialogueEditor.InitWindow(data);
            return true;
        }
        return false;
    }

    //选择改变时调用一次
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
        Repaint();  //重新绘制一次，强制执行OnGUI
    }


    //绘制显示内容
    private void OnGUI()
    {
        if (currentData != null)
        {
            EditorGUILayout.LabelField(currentData.name, EditorStyles.boldLabel);    //画出当前数据名称-粗体显示
            GUILayout.Space(10);                                                    //间距

            //绘制滑动条-随着滑动条的滑动随时调整坐标轴的文职
            scrollPos = GUILayout.BeginScrollView(scrollPos,GUILayout.ExpandWidth(true),GUILayout.ExpandHeight(true));

            if (pieceList == null)                                                  //判空后绘画新的列表
                SetupReorderableList();
            pieceList.DoLayoutList();
            GUILayout.EndScrollView();
        }
        else
        {
            if (GUILayout.Button("Create New Dialogue-创建一个新的对话"))   
            {
                string dataPath = "Assets/GameData/Dialogue Data/";
                if (!Directory.Exists(dataPath))
                {
                    Directory.CreateDirectory(dataPath);
                }
                DialogueData_SO newData = ScriptableObject.CreateInstance<DialogueData_SO>();   //创建一个数据
                AssetDatabase.CreateAsset(newData,dataPath + "/" + "New Dialogue.asset");       //穿件文件，把路径与数据传入
                currentData = newData;
            }
            GUILayout.Label("No Data Selected!", EditorStyles.boldLabel);
        }
    }

    private void OnDisable()    //字典用完后清空临时数据
    {
        optionListDict.Clear();
    }

    private void SetupReorderableList()
    {
        //绘制列表-后面4个true表示是否可编辑
        pieceList = new ReorderableList(currentData.dialoguePieces,typeof(DialoguePiece),true,true,true,true);

        //构建各个元素
        pieceList.drawHeaderCallback += OnDrawPieceHeader;
        pieceList.drawElementCallback += OnDrawPieceListElement;
        pieceList.elementHeightCallback += OnHeightChanged;             //高度更改
    }

    private float OnHeightChanged(int index)
    {
        return GetPieceHeight(currentData.dialoguePieces[index]);
    }

    //获取需要增加的高度
    float GetPieceHeight(DialoguePiece piece)
    {
        var height = EditorGUIUtility.singleLineHeight;

        var isExpand = piece.canExpand;
        if (isExpand)
        {

            height += EditorGUIUtility.singleLineHeight * 9;

            var options = piece.options;    //选项高度变化

            if (options.Count > 1)
            {
                height += EditorGUIUtility.singleLineHeight * options.Count;
            }
        }
        return height;

    }

    private void OnDrawPieceListElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        //标记后-实时更新修改内容
        EditorUtility.SetDirty(currentData);

        GUIStyle textStyle = new GUIStyle("TextField");

        if (index < currentData.dialoguePieces.Count)   //index-数据本身的index
        {
            var currentPiece = currentData.dialoguePieces[index];

            var tempRect = rect;    //临时大小-使用默认高度

            //绘制标签
            tempRect.height = EditorGUIUtility.singleLineHeight;

            currentPiece.canExpand = EditorGUI.Foldout(tempRect, currentPiece.canExpand, currentPiece.ID);
            if (currentPiece.canExpand)
            {
                tempRect.width = 30;
                tempRect.y += tempRect.height;
                EditorGUI.LabelField(tempRect, "ID");

                //绘制显示对话ID
                tempRect.x += tempRect.width;
                tempRect.width = 100;
                currentPiece.ID = EditorGUI.TextField(tempRect, currentPiece.ID);

                //绘制任务
                tempRect.x += tempRect.width + 10;
                EditorGUI.LabelField(tempRect, "Quest-任务");

                tempRect.x += 75;
                currentPiece.quest = (QuestData_SO)EditorGUI.ObjectField(tempRect, currentPiece.quest, typeof(QuestData_SO), false);

                //绘制图片
                tempRect.y += EditorGUIUtility.singleLineHeight + 5;
                tempRect.x = rect.x;
                tempRect.height = 60;
                tempRect.width = tempRect.height;
                currentPiece.image = (Sprite)EditorGUI.ObjectField(tempRect, currentPiece.image, typeof(Sprite), false);

                //绘制文本框
                tempRect.x += tempRect.width + 5;
                tempRect.width = rect.width - tempRect.x;
                textStyle.wordWrap = true;
                currentPiece.text = (string)EditorGUI.TextArea(tempRect, currentPiece.text, textStyle);

                //列表中画列表 -选项
                tempRect.y += tempRect.height + 5;
                tempRect.x = rect.x;
                tempRect.width = rect.width;


                //确定Key是否在字典中-否则创建新的列表放入字典
                string optionListKey = currentPiece.ID + currentPiece.text; //指针与内容双重确定

                if (optionListKey != null)
                {
                    if (!optionListDict.ContainsKey(optionListKey))
                    {   //如果字典内没有先创建列表
                        var optionList = new ReorderableList(currentPiece.options, typeof(DialogueOption), true, true, true, true);

                        optionList.drawHeaderCallback = OnDrawOptionHander;

                        optionList.drawElementCallback = (optionRect, optionIndex, optionActive, optionFocused) =>
                         {
                             OnDrawOptionElement(currentPiece,optionRect, optionIndex, optionActive, optionFocused);

                         };

                        //将列表保存到字典中
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
        GUI.Label(rect,"Dialogue Pieces 对话内容");
    }
}
