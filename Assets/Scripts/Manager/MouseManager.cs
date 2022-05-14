using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

//ϵͳ���л�-�༭����ʾ�¼�
[System.Serializable]
//public class EventVector3 : UnityEvent<Vector3> { }


public class MouseManager : Singleton<MouseManager>
{

    public Texture2D point, doorway, attack, target, arrow;

    RaycastHit hitInfo;                                             //������ײ����Ϣ

    public event Action<Vector3> OnMouseClicked;                    //������������¼�

    public event Action<GameObject> OnEnmyClicked;

    protected override void Awake()                                 //��Ҫ��awakeд�������ת���������뱻����
    {                   
        base.Awake();                                               //����ԭ�и�������ĺ���֮�ϣ���������
        DontDestroyOnLoad(this);
    }



    private void Update()
    {
        SetCursorTexture();
        if (InteractWithUI()) return;
        MouseControl();
    }

    //����ָ����ͼ
    void SetCursorTexture()
    {
        //�����Ļ���������ֻ����Ӧ��λ��
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (InteractWithUI())   
        {
            Cursor.SetCursor(point,  Vector2.zero, CursorMode.Auto);
            return;
        }

        if (Physics.Raycast(ray, out hitInfo))
        {
            //�л���ͼ
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target,new Vector2(16,16),CursorMode.Auto);     //ƫ��ֵ-Ҫ�滻��ͼƬ�ϴ�-��ʼ�ڹ�����Ͻǳ��֣���Ҫƫ�Ƶ��м�
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
            //��������桱���пգ�������Navgash��Ŀ������ΪhitInfo��λ��
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
