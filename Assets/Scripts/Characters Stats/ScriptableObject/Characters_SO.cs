using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//创建一个子集菜单-可以直接显示在新建中
[CreateAssetMenu(fileName = "New Datat", menuName = "Character State/Data")]
public class Characters_SO : ScriptableObject           //脚本对象-用于储存游戏数据
{
    [Header("State Info")]                              //隔开语句
    public int maxHealth;

    public int currentHealth;

    public int baseDefence;

    public int currentDefence;

    [Header("Kill")]

    public int killPoint;

    [Header("Level")]
    public int currentLevel;

    public int maxLevel;

    public int baseExp;

    public int currentExp;

    public float levelBuff;

    public float LevelMultiplier
    {
         get{ return 1 + (currentLevel - 1) * levelBuff; }
    }

    public void UpdateExp(int point)
    {
        currentExp += point;

        if (currentExp >= baseExp)
            LeveUp();
    }

    private void LeveUp()
    {
        //所有你想提升的数据方法
        currentLevel =  Mathf.Clamp( currentLevel + 1,0,maxLevel);      //最小值最大值之间，超过给予相应的最大、最小值
        baseExp += (int)(baseExp * LevelMultiplier);

        maxHealth = (int)(maxHealth * LevelMultiplier);
        currentHealth = maxHealth;

        Debug.Log("Level Up !" + currentLevel + "Max Health"+ maxHealth);

    }
}
