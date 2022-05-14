using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu (fileName = "New Attack",menuName = "Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange;

    public float skillRange;

    public float coolDown;

    public int minDamage;

    public int maxDamage;

    public float cirticalMultipliter;               //����

    public float criticalChance;                    //����

    //������������-weapon���´�������������
    public void ApplyWeapon(AttackData_SO weapon)
    {
        attackRange = weapon.attackRange;
        skillRange = weapon.skillRange;
        coolDown = weapon.coolDown;

        //TODO:ԭ�еĹ����������/ɾ��
        //minDamage += weapon.minDamage;

        //����ѧϰ����ʱֱ���滻ԭ�е�����
        minDamage = weapon.minDamage;
        maxDamage = weapon.maxDamage;

        criticalChance = weapon.criticalChance;
        cirticalMultipliter = weapon.cirticalMultipliter;
    }
}
