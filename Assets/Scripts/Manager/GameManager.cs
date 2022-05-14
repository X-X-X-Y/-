using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;              //其他脚本访问playerStats时通过GamManager访问

    private CinemachineFreeLook followCamera;           

    List<IEndGmaeObserver> endGmaeObservers = new List<IEndGmaeObserver>();     //接口列表

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void RigisterPlayer(CharacterStats player)                        //对playerStats赋值采用观察者模式
    {
        playerStats = player;

        followCamera = FindObjectOfType<CinemachineFreeLook>();

        if (followCamera != null)
        {
            followCamera.Follow = playerStats.transform.GetChild(2);
            followCamera.LookAt = playerStats.transform.GetChild(2);
        }
    }

    public void AddObsrever(IEndGmaeObserver observer)      //让敌人生成时自动加入列表
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
