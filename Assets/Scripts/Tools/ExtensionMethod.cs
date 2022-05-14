using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethod     //��̬��չ��������ʱ�ܹ�����
{

    private const float  dotThreshold = 0.5f;
    public static bool IsFacingTarget(this Transform transform, Transform target) //(this ��Ҫ��չ����, ��Ҫ���õı���)
    {
        

        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();

        float dot = Vector3.Dot(transform.forward, vectorToTarget);                 //�����ĵ�� = ������ģ֮�� * �����нǵ�����ֵ

        return dot >= dotThreshold;

        
    }
}
