using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Farm.Inventory
{
    [RequireComponent(typeof(SlotUI))]
    public class ActionBarButton : MonoBehaviour
    {
        public KeyCode key;
        private SlotUI slotUI;

        private void Awake()
        {
            slotUI = GetComponent<SlotUI>();

        }
        private void Update()
        {
            if (Input.GetKeyDown(key))
            {
                if(slotUI.itemDetails != null)
                {
                    slotUI.isSelected = !slotUI.isSelected;
                    if (slotUI.isSelected)
                    {
                        slotUI.inventoryUI.UpdateSlotHightlight(slotUI.slotIndex);
                    }
                    else
                    {
                        slotUI.inventoryUI.UpdateSlotHightlight(-1);
                    }
                    EventHandler.CallItemSelectedEvent(slotUI.itemDetails, slotUI.isSelected);
                }
            }
        }
    }

}
