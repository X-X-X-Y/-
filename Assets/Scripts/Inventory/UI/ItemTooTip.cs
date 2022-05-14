using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooTip : MonoBehaviour
{
    public Text itemName;
    public Text itemInfo;

    RectTransform rectTransform;                                    //��ȡUIλ��

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetupTooltip(ItemData_SO item)                      //���������Ϣ
    {
        itemName.text = item.itemName;
        itemInfo.text = item.description;
    }

    private void OnEnable()
    {
        UpdatePosition();       //��֤������ʾ��ʱ���ȸ�������
    }

    private void Update()
    {
        UpdatePosition();
    }

    public void UpdatePosition()            //ʵʱ����������
    {
        Vector3 mousPos = Input.mousePosition;

        Vector3[] coners = new Vector3[4];          //unity�Դ���ȥUI�ؼ���С����-���½ǵĵ�˳ʱ���ĸ����������λ��
        rectTransform.GetWorldCorners(coners);      //��ȡ�������

        float width = coners[3].x - coners[0].x;        //���㳤�� 
        float height = coners[1].y - coners[0].y;

        if (mousPos.y < height)                     //�ж����λ�����±߿�
            rectTransform.position = mousPos + Vector3.up * height * 0.6f;
        else if (Screen.width - mousPos.x > width)
            rectTransform.position = mousPos + Vector3.right * width * 0.6f;
        else
            rectTransform.position = mousPos + Vector3.left * width * 0.6f;
    }

}
