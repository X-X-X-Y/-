using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : Singleton<InventoryManager>
{

    public class DragData           //�н��ߣ�������ק��ʼ��Ϣ
    {
        public SlotHolder originalHolder;
        public RectTransform originalParent;
    }

    //TODO:������ģ�����ڱ������������ݿ�
    [Header("Inventory Data")]

    public InventoryData_SO inventoryTemplate;
    public InventoryData_SO inventoryData;

    public InventoryData_SO actionTemplate;
    public InventoryData_SO actionData;

    public InventoryData_SO equipmentTemplate;
    public InventoryData_SO equipmentData;


    [Header("ContainerS")]
    public ContainerUI inventoryUI;     //������ִ������ContainerUI

    public ContainerUI actionUI;

    public ContainerUI equipmentUI;

    [Header("Drag Canvas")]
    //��ק��Ʒʱ������ߵȼ�Canvas����Ⱦ
    public Canvas dragCanvas;
    public DragData currentDrag;        //��ʱ���浱ǰ��ק


    [Header("UI Panel")]
    public GameObject bagPanel;
    public GameObject statsPanel;


    bool isOpen = false;

    [Header("Stats Text")]
    public Text healthText;
    public Text attackText;

    [Header("Tooltip")]
    public ItemTooTip toolTip;           //ͨ��InteroryManager������Ϣ��ʾ���


    protected override void Awake()     //����ģʽ�±�����override
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
        //ˢ��UI-��Ϸ����ʱˢ�¶�ȡ���������Ϣ
        inventoryUI.RefreshUI();
        actionUI.RefreshUI();
        equipmentUI.RefreshUI();
    }

    //�����������
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
        if (Input.GetKeyDown(KeyCode.B))        //Ĭ�ϱ����ر�
        {
            isOpen = !isOpen;
            bagPanel.SetActive(isOpen);
            statsPanel.SetActive(isOpen);
        }
        //ͨ��GameManager��ȡ��ɫ��ǰ������ֵ�빥����
        UpdateStatsText(GameManager.Instance.playerStats.MaxHealth, GameManager.Instance.playerStats.attackData.minDamage, GameManager.Instance.playerStats.attackData.maxDamage);
    }

    public void UpdateStatsText(int health, int min, int max)               //��ȡ��ʵ�����Ϣ
    {
        healthText.text = health.ToString();
        attackText.text = min + " -" + max;
    }

    #region �����ק��Ʒ�Ƿ���ÿһ�� Slot �ķ�Χ��-���ֵĵط��Ƿ��Ǹ�����
    public bool CheckInInventoryUI(Vector3 position)                                  //�������λ��
    {
        for (int i = 0; i < inventoryUI.slotHolders.Length; i++)                        //��������inventoryUI�µĸ���
        {
            RectTransform t = inventoryUI.slotHolders[i].transform as RectTransform;    //�Ƚ����ӵ�λ��ת��RectTransform����t

            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))          //unity���Դ������Ƿ����趨�ľ��η�Χ���ж�
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


    #region ��鱳�������������Ʒ�Ƿ�������������

    public void CheckQuestItemInBag(string questItemName)
    {
        foreach (var item in inventoryData.items)   //ѭ�������е�������Ʒ
        {
            if (item.itemData != null)              //���п�-�ж���û����Ʒ
            {
                if (item.itemData.itemName == questItemName)  //�ж��Ƿ�������������Ʒ
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

    #region ��鱳���������Ʒ-�۳������������Ʒ

    public InventoryItem QuestItemInBag(ItemData_SO questItem)  //��鱳��-���ر�����Ʒ
    {
        return inventoryData.items.Find(i =>i.itemData == questItem);   //�Ӵ������Ʒ��Ϣ���������������Ʒ
    }
    public InventoryItem QuestItemInAction(ItemData_SO questItem)  
    {
        return actionData.items.Find(i => i.itemData == questItem);  
    }

    #endregion


}
