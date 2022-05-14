using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethod     //静态拓展方法，随时能够调用
{

    private const float  dotThreshold = 0.5f;
    public static bool IsFacingTarget(this Transform transform, Transform target) //(this 需要扩展的类, 需要调用的变量)
    {
        

        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();

        float dot = Vector3.Dot(transform.forward, vectorToTarget);                 //向量的点积 = 向量的模之积 * 向量夹角的余弦值

        return dot >= dotThreshold;

        
    }
}
