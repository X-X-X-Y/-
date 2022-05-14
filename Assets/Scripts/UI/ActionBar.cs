using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBar : MonoBehaviour
{
    public KeyCode actionKey;

    private SlotHolder currentSlotHold;             //�õ���ǰ�ĸ���

    private void Awake()
    {
        currentSlotHold = GetComponent<SlotHolder>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(actionKey) && currentSlotHold.itemUI.GetItem())
        {
            currentSlotHold.UseItem();
        }
    }

}
