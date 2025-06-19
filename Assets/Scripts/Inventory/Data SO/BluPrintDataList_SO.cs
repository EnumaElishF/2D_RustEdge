using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BluPrintDataList_SO",menuName = "Inventory/BluPrintDataList_SO")]
public class BluPrintDataList_SO : ScriptableObject
{
    public List<BluePrintDetails> bluePrintDataList;
    public BluePrintDetails GetBluePrintDetails(int itemID)
    {
        return bluePrintDataList.Find(b => b.ID == itemID);
    }
}
[System.Serializable]
public class BluePrintDetails
{
    public int ID;
    public InventoryItem[] resourceItem = new InventoryItem[4];
    public GameObject buildPrefab;

}
