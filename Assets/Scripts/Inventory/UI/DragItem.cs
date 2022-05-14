using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemUI))]          //运用在其他组件中确保能够获取
public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //使用接口实现背包的拖拽功能 EventSystem

    ItemUI currentItemUI;       //获取ItemUI中当前物品信息
    SlotHolder currentHolder;   //通过中间者交换数据-获取当前格子/目标格子信息
    SlotHolder targetHolder;

    private void Awake()
    {
        //获取单签格子及物品信息
        currentItemUI = GetComponent<ItemUI>();
        currentHolder = GetComponentInParent<SlotHolder>();
    }

    public void OnBeginDrag(PointerEventData eventData)     //PointerEventData-与指针/鼠标/触摸 事件关联的有效负载
    {
        InventoryManager.Instance.currentDrag = new InventoryManager.DragData();                //赋值拖拽物品的数据
        InventoryManager.Instance.currentDrag.originalHolder = GetComponent<SlotHolder>();
        InventoryManager.Instance.currentDrag.originalParent = (RectTransform)transform.parent;
        

        //鼠标点击-开始拖拽-记录原始信息
        transform.SetParent(InventoryManager.Instance.dragCanvas.transform,true);   //开始拖拽时将父级渲染设置成dragCanvas

    }

    public void OnDrag(PointerEventData eventData)
    {
        //拖拽中-跟随鼠标移动
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //松开左键-结束拖拽-放下物品-结束拖拽
        
        if (EventSystem.current.IsPointerOverGameObject())                  //拖拽指向是否为UI坐标系IsPointerOverGameObject-对三个部分的UI分别检测
        {
            if (InventoryManager.Instance.CheckActionUI(eventData.position) ||
                InventoryManager.Instance.CheckEquipmentUI(eventData.position)||
                InventoryManager.Instance.CheckInInventoryUI(eventData.position))
            {
                //因为拖拽的时候被拖拽的物品会被放进显示的Canvas
                if (eventData.pointerEnter.gameObject.GetComponent<SlotHolder>())   //判断鼠标进入的格子里面有没有SlotHolder
                    targetHolder = eventData.pointerEnter.gameObject.GetComponent<SlotHolder>(); //将当前的SlotHolder给到目标
                else
                    targetHolder = eventData.pointerEnter.gameObject.GetComponentInParent<SlotHolder>();    //去父级找SlotHolder

                //判断是否是原先的Hold
                if (targetHolder != InventoryManager.Instance.currentDrag.originalHolder)
                    switch (targetHolder.slotType)          //判断道具进入指定的格子
                {
                    case SlotType.BAG:
                        SwapItem();
                        break;
                    case SlotType.WEAPON:
                        if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Weapon)
                            SwapItem();
                        break;
                    case SlotType.ARMOR:
                        if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Armor)
                            SwapItem();
                        break;
                    case SlotType.ACTION:
                        if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Useable)     //通过UI判断当前拖拽物品的类型
                            SwapItem();
                        break;
                }

                currentHolder.UpdateItem();
                targetHolder.UpdateItem();
            }
        }
        transform.SetParent(InventoryManager.Instance.currentDrag.originalParent);        //拖拽的格子结束后回到自己的父级-因为只是改变对应格子的物品信息

        RectTransform t = transform as RectTransform;                                     //回到原来的格子里

        t.offsetMax = -Vector2.one * 1;                   //RectTransform右上角距离锚点的距离
        t.offsetMin = -Vector2.one * 1;
    }

    public void SwapItem()      //交换物品
    {
        //设定目标与需要拖拽的物品
        var targetItem = targetHolder.itemUI.Bag.items[targetHolder.itemUI.Index];        //改变格子顺序，达到拖拽物品效果
        var tempItem = currentHolder.itemUI.Bag.items[currentHolder.itemUI.Index];        //当前拖拽的物品

        bool isSameItem = tempItem.itemData == targetItem.itemData;                       //判断拖拽物品与目标格子的物品是否相同

        if (isSameItem && targetItem.itemData.stackable)                                  //同时满足相同物品且可以叠加
        {
            targetItem.amount += tempItem.amount;                                         //完成数据添加
            targetItem.itemData = tempItem.itemData;
            tempItem.amount = 0;                                                          //完成数据添加后，清空拖拽数据

        }
        else
        {
            currentHolder.itemUI.Bag.items[currentHolder.itemUI.Index] = targetItem;       //否则直将目标与拖拽的物品相互交换
            targetHolder.itemUI.Bag.items[targetHolder.itemUI.Index] = tempItem;
        }
    }
}
