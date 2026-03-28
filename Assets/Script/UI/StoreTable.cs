using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 商店物品配置表 - 存储商店物品的静态配置
/// </summary>
[CreateAssetMenu(menuName = "StoreTable", fileName = "StoreTable")]
public class StoreTable : ScriptableObject
{
    public List<StoreTableItem> DataList = new List<StoreTableItem>();
}

/// <summary>
/// 商店物品配置项
/// </summary>
[System.Serializable]
public class StoreTableItem
{
    public int id;
    public string name;
    public Sprite sprite;
    public int level;
    public ItemType itemType;
}