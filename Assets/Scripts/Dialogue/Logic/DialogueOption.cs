using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueOption 
{
    public string text;
    public string targetID;       //需要跳转到哪一条
    public bool takeQuest;

}
