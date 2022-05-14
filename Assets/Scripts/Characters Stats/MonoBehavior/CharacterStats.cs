using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public event Action<int ,int > UpdateHealthBarOnAttack;         //当前血量与最大血量

    public Characters_SO templateData;          //模板数据，给每一个游戏体单独的脚本对象

    public Characters_SO characterData;         //读取脚本对象/数据模板

    public AttackData_SO attackData;
    private AttackData_SO baseAttackData;       //游戏一开始给玩家基础攻击力

    private RuntimeAnimatorController baseAnimator; //保留初始动画

    [Header("Weapon")]
    public Transform weaponSlot;


    [HideInInspector]       //不需要在窗口显示
    public bool isCritical;

    private void Awake()
    {   //先判空-再次生成一个模板数据
        if (templateData != null)
         characterData = Instantiate(templateData);

        baseAttackData = Instantiate(attackData);
        baseAnimator = GetComponent<Animator>().runtimeAnimatorController;      //获取原有动画
    }

    #region Read from Data_SO 读取Data_SO数据
    public int MaxHealth            //设置属性
    {
        get
        { if (characterData != null)    return characterData.maxHealth;   else return 0;   }
        set 
        {  characterData.maxHealth = value;   }
    
    }
    public int CurrentHealth
    {
        get
        { if (characterData != null) return characterData.currentHealth; else return 0; }
        set
        { characterData.currentHealth = value; }
    }
    public int BaseDefence
    {
        get
        { if (characterData != null) return characterData.baseDefence; else return 0; }
        set
        { characterData.baseDefence = value; }
    }
    public int CurrentDefence
    {
        get
        { if (characterData != null) return characterData.currentDefence; else return 0; }
        set
        { characterData.currentDefence = value; }
    }
    #endregion

    #region Character Combat 伤害数值计算

    public void TakeDamage(CharacterStats attacker , CharacterStats defener)
    {
        //需要考虑防御大于攻击，导致攻击成加血了
        int damage =Mathf.Max (attacker.CurrentDamage() - defener.CurrentDefence,0);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        if (attacker.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");     //直接收到暴击的角色播放相应动画
        }
        //TODO: UI
        //判空
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth,MaxHealth);

        //TODO: 经验值
        //无论是谁，只要死亡，经验值加给攻击者的身上
        if (CurrentHealth <= 0)
            attacker.characterData.UpdateExp(characterData.killPoint); 

    }

    //函数的重载-相同函数名称执行不同指令
    public void TakeDamage(int damage,CharacterStats defener)
    {
        int currentDamage = Mathf.Max(damage - defener.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamage, 0);
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0)
            GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint);
    }

    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);

        if (isCritical)
        {
            coreDamage *= attackData.cirticalMultipliter;
            Debug.Log("暴击" + coreDamage);
        }

        return (int)coreDamage;
    }

    #endregion

    #region Equip Weapon 装备武器

    public void ChangeWeapon(ItemData_SO weapon)
    {
        UnEquipWeapon();
        EquipWeapon(weapon);
    }

    public void EquipWeapon( ItemData_SO weapon )
    {
        if (weapon.weaponPrefab != null)
        {
            //weaponSlot提供父级坐标位置-生成武器的预支体
            Instantiate(weapon.weaponPrefab,weaponSlot);
        }
        //TODO:更新属性
        //切换人物动画
        attackData.ApplyWeapon(weapon.weaponData);
        GetComponent<Animator>().runtimeAnimatorController = weapon.weaponAnimtor;      //runtimeAnimatorController实时动画控制器


        //InventoryManager.Instance.UpdateStatsText(MaxHealth,attackData.minDamage,attackData.maxDamage);
    }

    public void UnEquipWeapon()                                             //卸载武器
    {
        if (weaponSlot.transform.childCount != 0)                           //childCount = 0 没有-判断是否有武器
        {
            for (int i = 0; i < weaponSlot.transform.childCount; i++)
            {
                Destroy(weaponSlot.transform.GetChild(i).gameObject);
            }
        }

        attackData.ApplyWeapon(baseAttackData);                             //卸载武器还原之前的攻击、动画数据
        GetComponent<Animator>().runtimeAnimatorController = baseAnimator;
    }

    #endregion

    #region   Apply Data Change -使用消耗物品时

    public void ApplyHealth( int amount )
    {
        if (CurrentHealth + amount <= MaxHealth)
        {
            CurrentHealth += amount;
        }
        else
        {
            CurrentHealth = MaxHealth;
        }
    }


    #endregion
}
