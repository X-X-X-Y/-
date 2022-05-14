using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooTip : MonoBehaviour
{
    public Text itemName;
    public Text itemInfo;

    RectTransform rectTransform;                                    //获取UI位置

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetupTooltip(ItemData_SO item)                      //设置相关信息
    {
        itemName.text = item.itemName;
        itemInfo.text = item.description;
    }

    private void OnEnable()
    {
        UpdatePosition();       //保证启动显示的时候，先更新坐标
    }

    private void Update()
    {
        UpdatePosition();
    }

    public void UpdatePosition()            //实时获得鼠标坐标
    {
        Vector3 mousPos = Input.mousePosition;

        Vector3[] coners = new Vector3[4];          //unity自带过去UI控件大小方法-左下角的点顺时针四个顶点的坐标位置
        rectTransform.GetWorldCorners(coners);      //获取这个数组

        float width = coners[3].x - coners[0].x;        //计算长宽 
        float height = coners[1].y - coners[0].y;

        if (mousPos.y < height)                     //判断鼠标位置离下边框
            rectTransform.position = mousPos + Vector3.up * height * 0.6f;
        else if (Screen.width - mousPos.x > width)
            rectTransform.position = mousPos + Vector3.right * width * 0.6f;
        else
            rectTransform.position = mousPos + Vector3.left * width * 0.6f;
    }

}
