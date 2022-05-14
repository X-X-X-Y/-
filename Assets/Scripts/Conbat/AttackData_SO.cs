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

    public float cirticalMultipliter;               //爆伤

    public float criticalChance;                    //爆率

    //更新武器属性-weapon更新传入武器的属性
    public void ApplyWeapon(AttackData_SO weapon)
    {
        attackRange = weapon.attackRange;
        skillRange = weapon.skillRange;
        coolDown = weapon.coolDown;

        //TODO:原有的攻击力上添加/删减
        //minDamage += weapon.minDamage;

        //方便学习。暂时直接替换原有的属性
        minDamage = weapon.minDamage;
        maxDamage = weapon.maxDamage;

        criticalChance = weapon.criticalChance;
        cirticalMultipliter = weapon.cirticalMultipliter;
    }
}
