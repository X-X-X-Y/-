using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestRequirement : MonoBehaviour
{
    private Text requireName;                   //需求名称
    private Text progressNumber;                //需求进度


    private void Awake()                        //拿到自身、子物体上的变量
    {
        requireName = GetComponent<Text>();
        progressNumber = transform.GetChild(0).GetComponent<Text>();
    }

    //设置任务需求及其进度
    public void SetupRequirement( string name , int amount , int currentAmount )
    {
        requireName.text = name;
        progressNumber.text = currentAmount.ToString() + " /" + amount.ToString();
    }

    //任务结束后显示
    public void SetupRequirement( string name , bool isFinished )
    {
        if (isFinished)
        {
            requireName.text = name;
            progressNumber.text = "完成";
            requireName.color = Color.gray;
            progressNumber.color = Color.gray;
        }
    }

}
