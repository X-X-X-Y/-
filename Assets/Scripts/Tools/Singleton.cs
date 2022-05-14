using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>        //泛型单例-包含多个Manager-T:Type-where:约束
{
    private static T instance;


    public static T Instance
    {
        get { return instance; }
    }

    protected virtual void Awake()      //protected只允许继承的类能够访问 virtual:在继承函数中可以重写
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = (T)this;
    }

    public static bool IsInitialized    //获取当前单例是否初始化生成
    {
        get { return instance != null; }
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
