using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ItemToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Text valueText;
    [SerializeField] private GameObject bottomPart;

    //为了能够在买卖物品时，买入商店的价格是全价，卖出价格按itemDetails的sellPercentage进行比例折损，需要SlotType判断插槽类型，包括背包，容器，商店，等
    public void SetupTooltip(ItemDetails itemDetails,SlotType slotType)
    {
        nameText.text = itemDetails.itemName;
        typeText.text = GetItemType(itemDetails.itemType);
        descriptionText.text = itemDetails.itemDescription;

        if(itemDetails.itemType == ItemType.Seed || itemDetails.itemType == ItemType.Commodity || itemDetails.itemType == ItemType.Furniture)
        {
            bottomPart.SetActive(true);
            var price = itemDetails.itemPrice;
            if (slotType == SlotType.Bag)
            {
                //背包价格：显示折损售出价格
                price = (int)(price * itemDetails.sellPercentage);
            }
            valueText.text = price.ToString();
        }
        else
        {
            //如果是种子，货物，家具的其他类型，这种没有金币价格，那么就隐藏boomPart
            bottomPart.SetActive(false);
        }
        //防止Description从一行变成多行造成的延迟
        //LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
    private string GetItemType(ItemType itemType)
    {
        //C#新的快捷语法糖
        return itemType switch
        {
            ItemType.Seed => "种子",
            ItemType.Commodity => "商品",
            ItemType.Furniture => "家具",
            ItemType.BreakTool => "工具",
            ItemType.ChopTool => "工具",
            ItemType.CollectTool => "工具",
            ItemType.HoeTool => "工具",
            ItemType.ReapTool => "工具",
            ItemType.WaterTool => "工具",
            _ => "无"

        };
    }
}
