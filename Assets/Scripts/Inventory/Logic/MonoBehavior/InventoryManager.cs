using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : Singleton<InventoryManager>
{

    public class DragData           //中介者，保存拖拽起始信息
    {
        public SlotHolder originalHolder;
        public RectTransform originalParent;
    }

    //TODO:最后添加模板用于保存数据至数据库
    [Header("Inventory Data")]

    public InventoryData_SO inventoryTemplate;
    public InventoryData_SO inventoryData;

    public InventoryData_SO actionTemplate;
    public InventoryData_SO actionData;

    public InventoryData_SO equipmentTemplate;
    public InventoryData_SO equipmentData;


    [Header("ContainerS")]
    public ContainerUI inventoryUI;     //管理器执行所有ContainerUI

    public ContainerUI actionUI;

    public ContainerUI equipmentUI;

    [Header("Drag Canvas")]
    //拖拽物品时处于最高等级Canvas中渲染
    public Canvas dragCanvas;
    public DragData currentDrag;        //临时保存当前拖拽


    [Header("UI Panel")]
    public GameObject bagPanel;
    public GameObject statsPanel;


    bool isOpen = false;

    [Header("Stats Text")]
    public Text healthText;
    public Text attackText;

    [Header("Tooltip")]
    public ItemTooTip toolTip;           //通过InteroryManager控制信息显示面板


    protected override void Awake()     //单例模式下必须是override
    {
        base.Awake();
        if (inventoryTemplate != null)
            inventoryData = Instantiate(inventoryTemplate);
        if (actionTemplate != null)
            actionData = Instantiate(actionTemplate);
        if (equipmentTemplate != null)
            equipmentData = Instantiate(equipmentTemplate);
    }
    private void Start()
    {
        LoadData();
        //刷新UI-游戏进入时刷新读取都的玩家信息
        inventoryUI.RefreshUI();
        actionUI.RefreshUI();
        equipmentUI.RefreshUI();
    }

    //保存加载数据
    public void SaveData()
    {
        SaveManager.Instance.Save(inventoryData, inventoryData.name);
        SaveManager.Instance.Save(actionData, actionData.name);
        SaveManager.Instance.Save(equipmentData, equipmentData.name);
    }

    public void LoadData()
    {
        SaveManager.Instance.Load(inventoryData, inventoryData.name);
        SaveManager.Instance.Load(actionData, actionData.name);
        SaveManager.Instance.Load(equipmentData, equipmentData.name);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))        //默认背包关闭
        {
            isOpen = !isOpen;
            bagPanel.SetActive(isOpen);
            statsPanel.SetActive(isOpen);
        }
        //通过GameManager获取角色当前的生命值与攻击力
        UpdateStatsText(GameManager.Instance.playerStats.MaxHealth, GameManager.Instance.playerStats.attackData.minDamage, GameManager.Instance.playerStats.attackData.maxDamage);
    }

    public void UpdateStatsText(int health, int min, int max)               //获取真实玩家信息
    {
        healthText.text = health.ToString();
        attackText.text = min + " -" + max;
    }

    #region 检查拖拽物品是否在每一个 Slot 的范围内-放手的地方是否是格子里
    public bool CheckInInventoryUI(Vector3 position)                                  //获得鼠标的位置
    {
        for (int i = 0; i < inventoryUI.slotHolders.Length; i++)                        //遍历所有inventoryUI下的格子
        {
            RectTransform t = inventoryUI.slotHolders[i].transform as RectTransform;    //先将格子的位置转换RectTransform传给t

            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))          //unity中自带对于是否在设定的矩形范围内判断
            {
                return true;
            }
        }
        return false;
    }
    public bool CheckActionUI(Vector3 position)
    {
        for (int i = 0; i < actionUI.slotHolders.Length; i++)
        {
            RectTransform t = actionUI.slotHolders[i].transform as RectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckEquipmentUI(Vector3 position)
    {
        for (int i = 0; i < equipmentUI.slotHolders.Length; i++)
        {
            RectTransform t = equipmentUI.slotHolders[i].transform as RectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }
        return false;
    }


    #endregion


    #region 检查背包、快捷栏中物品是否满足任务需求

    public void CheckQuestItemInBag(string questItemName)
    {
        foreach (var item in inventoryData.items)   //循环背包中的所有物品
        {
            if (item.itemData != null)              //先判空-判断有没有物品
            {
                if (item.itemData.itemName == questItemName)  //判断是否有任务需求物品
                    QuestManager.Instance.UpdateQuestProgress(item.itemData.itemName, item.amount);
            }
        }

        foreach (var item in actionData.items)
        {
            if (item.itemData != null)
            {
                if (item.itemData.itemName == questItemName)
                    QuestManager.Instance.UpdateQuestProgress(item.itemData.itemName, item.amount);
            }
        }
    }

    #endregion

    #region 检查背包快捷栏物品-扣除任务需求的物品

    public InventoryItem QuestItemInBag(ItemData_SO questItem)  //检查背包-返回背包物品
    {
        return inventoryData.items.Find(i =>i.itemData == questItem);   //从传入的物品信息查找任务需求的物品
    }
    public InventoryItem QuestItemInAction(ItemData_SO questItem)  
    {
        return actionData.items.Find(i => i.itemData == questItem);  
    }

    #endregion


}
