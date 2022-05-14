using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>        //���͵���-�������Manager-T:Type-where:Լ��
{
    private static T instance;


    public static T Instance
    {
        get { return instance; }
    }

    protected virtual void Awake()      //protectedֻ����̳е����ܹ����� virtual:�ڼ̳к����п�����д
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = (T)this;
    }

    public static bool IsInitialized    //��ȡ��ǰ�����Ƿ��ʼ������
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
