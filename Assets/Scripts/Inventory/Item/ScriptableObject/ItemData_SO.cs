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

    public int itemAmount;  //数量-当前物品本身的数量

    //物品的描述
    [TextArea]
    public string description = "";
    //判断是否可以堆叠-物资/武器
    public bool stackable;
    [Header("Userable Item")]

    public UserableItemData_SO useableData;


    [Header("Weapon")]
    public GameObject weaponPrefab;

    public AttackData_SO weaponData;

    public AnimatorOverrideController weaponAnimtor;                //配套武器的动画控制器
}
