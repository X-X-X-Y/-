using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Useable,
    Weapon,
    Armor
}

[CreateAssetMenu(fileName ="New Item",menuName = "Inventory/Item Data")]
public class ItemData_SO : ScriptableObject
{
    public ItemType itemType;

    public string itemName;

    public Sprite itemIcon;

    public int itemAmount;  //����-��ǰ��Ʒ���������

    //��Ʒ������
    [TextArea]
    public string description = "";
    //�ж��Ƿ���Զѵ�-����/����
    public bool stackable;
    [Header("Userable Item")]

    public UserableItemData_SO useableData;


    [Header("Weapon")]
    public GameObject weaponPrefab;

    public AttackData_SO weaponData;

    public AnimatorOverrideController weaponAnimtor;                //���������Ķ���������
}
