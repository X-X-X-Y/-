using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public event Action<int ,int > UpdateHealthBarOnAttack;         //��ǰѪ�������Ѫ��

    public Characters_SO templateData;          //ģ�����ݣ���ÿһ����Ϸ�嵥���Ľű�����

    public Characters_SO characterData;         //��ȡ�ű�����/����ģ��

    public AttackData_SO attackData;
    private AttackData_SO baseAttackData;       //��Ϸһ��ʼ����һ���������

    private RuntimeAnimatorController baseAnimator; //������ʼ����

    [Header("Weapon")]
    public Transform weaponSlot;


    [HideInInspector]       //����Ҫ�ڴ�����ʾ
    public bool isCritical;

    private void Awake()
    {   //���п�-�ٴ�����һ��ģ������
        if (templateData != null)
         characterData = Instantiate(templateData);

        baseAttackData = Instantiate(attackData);
        baseAnimator = GetComponent<Animator>().runtimeAnimatorController;      //��ȡԭ�ж���
    }

    #region Read from Data_SO ��ȡData_SO����
    public int MaxHealth            //��������
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

    #region Character Combat �˺���ֵ����

    public void TakeDamage(CharacterStats attacker , CharacterStats defener)
    {
        //��Ҫ���Ƿ������ڹ��������¹����ɼ�Ѫ��
        int damage =Mathf.Max (attacker.CurrentDamage() - defener.CurrentDefence,0);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        if (attacker.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");     //ֱ���յ������Ľ�ɫ������Ӧ����
        }
        //TODO: UI
        //�п�
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth,MaxHealth);

        //TODO: ����ֵ
        //������˭��ֻҪ����������ֵ�Ӹ������ߵ�����
        if (CurrentHealth <= 0)
            attacker.characterData.UpdateExp(characterData.killPoint); 

    }

    //����������-��ͬ��������ִ�в�ָͬ��
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
            Debug.Log("����" + coreDamage);
        }

        return (int)coreDamage;
    }

    #endregion

    #region Equip Weapon װ������

    public void ChangeWeapon(ItemData_SO weapon)
    {
        UnEquipWeapon();
        EquipWeapon(weapon);
    }

    public void EquipWeapon( ItemData_SO weapon )
    {
        if (weapon.weaponPrefab != null)
        {
            //weaponSlot�ṩ��������λ��-����������Ԥ֧��
            Instantiate(weapon.weaponPrefab,weaponSlot);
        }
        //TODO:��������
        //�л����ﶯ��
        attackData.ApplyWeapon(weapon.weaponData);
        GetComponent<Animator>().runtimeAnimatorController = weapon.weaponAnimtor;      //runtimeAnimatorControllerʵʱ����������


        //InventoryManager.Instance.UpdateStatsText(MaxHealth,attackData.minDamage,attackData.maxDamage);
    }

    public void UnEquipWeapon()                                             //ж������
    {
        if (weaponSlot.transform.childCount != 0)                           //childCount = 0 û��-�ж��Ƿ�������
        {
            for (int i = 0; i < weaponSlot.transform.childCount; i++)
            {
                Destroy(weaponSlot.transform.GetChild(i).gameObject);
            }
        }

        attackData.ApplyWeapon(baseAttackData);                             //ж��������ԭ֮ǰ�Ĺ�������������
        GetComponent<Animator>().runtimeAnimatorController = baseAnimator;
    }

    #endregion

    #region   Apply Data Change -ʹ��������Ʒʱ

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
