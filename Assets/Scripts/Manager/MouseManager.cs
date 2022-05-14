using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

//系统序列化-编辑器显示事件
[System.Serializable]
//public class EventVector3 : UnityEvent<Vector3> { }


public class MouseManager : Singleton<MouseManager>
{

    public Texture2D point, doorway, attack, target, arrow;

    RaycastHit hitInfo;                                             //射线碰撞的信息

    public event Action<Vector3> OnMouseClicked;                    //定义鼠标点击的事件

    public event Action<GameObject> OnEnmyClicked;

    protected override void Awake()                                 //需要在awake写入变量，转换场景不想被销毁
    {                   
        base.Awake();                                               //基于原有父类里面的函数之上，额外运行
        DontDestroyOnLoad(this);
    }



    private void Update()
    {
        SetCursorTexture();
        if (InteractWithUI()) return;
        MouseControl();
    }

    //设置指针贴图
    void SetCursorTexture()
    {
        //点击屏幕后相机射线只想相应的位置
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (InteractWithUI())   
        {
            Cursor.SetCursor(point,  Vector2.zero, CursorMode.Auto);
            return;
        }

        if (Physics.Raycast(ray, out hitInfo))
        {
            //切换贴图
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target,new Vector2(16,16),CursorMode.Auto);     //偏移值-要替换的图片较大-初始在光标左上角出现，需要偏移到中间
                    break;
                case "enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Protal":
                    Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Item":
                    Cursor.SetCursor(point, new Vector2(16, 16), CursorMode.Auto);
                    break;

                default:
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);
                    break;

            }
        }
    }

    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            //点击“地面”后，判空，将人物Navgash的目标设置为hitInfo的位置
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
                OnMouseClicked?.Invoke(hitInfo.point);
            if (hitInfo.collider.gameObject.CompareTag("enemy"))
                OnEnmyClicked?.Invoke(hitInfo.collider.gameObject);
            if (hitInfo.collider.gameObject.CompareTag("Attackable"))
                OnEnmyClicked?.Invoke(hitInfo.collider.gameObject);
            if (hitInfo.collider.gameObject.CompareTag("Protal"))
                OnMouseClicked?.Invoke(hitInfo.point);
            if (hitInfo.collider.gameObject.CompareTag("Item"))
                OnMouseClicked?.Invoke(hitInfo.point);
        }
    }

    bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}
