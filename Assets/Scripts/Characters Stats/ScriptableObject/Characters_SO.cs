using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//����һ���Ӽ��˵�-����ֱ����ʾ���½���
[CreateAssetMenu(fileName = "New Datat", menuName = "Character State/Data")]
public class Characters_SO : ScriptableObject           //�ű�����-���ڴ�����Ϸ����
{
    [Header("State Info")]                              //�������
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
        //�����������������ݷ���
        currentLevel =  Mathf.Clamp( currentLevel + 1,0,maxLevel);      //��Сֵ���ֵ֮�䣬����������Ӧ�������Сֵ
        baseExp += (int)(baseExp * LevelMultiplier);

        maxHealth = (int)(maxHealth * LevelMultiplier);
        currentHealth = maxHealth;

        Debug.Log("Level Up !" + currentLevel + "Max Health"+ maxHealth);

    }
}
