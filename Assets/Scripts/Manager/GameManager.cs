using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;              //�����ű�����playerStatsʱͨ��GamManager����

    private CinemachineFreeLook followCamera;           

    List<IEndGmaeObserver> endGmaeObservers = new List<IEndGmaeObserver>();     //�ӿ��б�

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void RigisterPlayer(CharacterStats player)                        //��playerStats��ֵ���ù۲���ģʽ
    {
        playerStats = player;

        followCamera = FindObjectOfType<CinemachineFreeLook>();

        if (followCamera != null)
        {
            followCamera.Follow = playerStats.transform.GetChild(2);
            followCamera.LookAt = playerStats.transform.GetChild(2);
        }
    }

    public void AddObsrever(IEndGmaeObserver observer)      //�õ�������ʱ�Զ������б�
    {
        endGmaeObservers.Add(observer);
    }

    public void RemoveObserver(IEndGmaeObserver observer)
    {
        endGmaeObservers.Remove(observer);
    }

    public void NotifyObserver()
    {
        foreach (var observer in endGmaeObservers)
        {
            observer.EndNotify();
        }
    }

    public Transform GetEntrance()
    {
        foreach (var item in FindObjectsOfType<TranstionDestination>())
        {
            if (item.destinationTag == TranstionDestination.DestinationTag.ENTER)
                return item.transform;
        }
        return null;
    }
    
}
