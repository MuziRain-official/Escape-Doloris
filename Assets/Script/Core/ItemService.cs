using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物品服务层 - 统一管理物品相关的数据访问
/// 作用：解耦UI层与业务层，UI通过服务层访问数据，而不直接调用GameManager
/// </summary>
public class ItemService
{
    private static ItemService _instance;
    public static ItemService Instance => _instance ??= new ItemService();

    private ItemService() { }

    // ==================== 静态表数据访问 ====================

    /// <summary>获取物品静态配置</summary>
    public PackageTableItem GetTableItem(int id)
    {
        return GameManager.Instance.GetPackageItemById(id);
    }

    /// <summary>获取所有物品配置列表</summary>
    public List<PackageTableItem> GetAllTableItems()
    {
        return GameManager.Instance.GetPackageTable()?.DataList;
    }

    // ==================== 背包数据访问 ====================

    /// <summary>获取原始背包数据</summary>
    public List<PackageLocalItem> GetRawPackageData()
    {
        return GameManager.Instance.GetPackageLocalData();
    }

    /// <summary>获取处理后的背包数据（装备单独显示，素材合并显示）</summary>
    public List<PackageLocalItem> GetProcessedPackageData()
    {
        return GameManager.Instance.GetProcessedPackageData();
    }

    /// <summary>根据UID获取物品实例</summary>
    public PackageLocalItem GetPackageItemByUid(string uid)
    {
        return GameManager.Instance.GetPackageLocalItemByUId(uid);
    }

    /// <summary>获取排序后的背包数据</summary>
    public List<PackageLocalItem> GetSortedPackageData()
    {
        return GameManager.Instance.GetSortPackageLocalData();
    }

    // ==================== 背包操作 ====================

    /// <summary>添加物品到背包</summary>
    public void AddItem(int itemId, int count = 1)
    {
        GameManager.Instance.AddItemToPackage(itemId, count);
    }

    /// <summary>移除物品（出售时使用）</summary>
    public void RemoveItem(string uid, int count = 1)
    {
        var item = GetPackageItemByUid(uid);
        if (item == null) return;

        var rawData = GetRawPackageData();
        item.num -= count;

        if (item.num <= 0)
        {
            rawData.Remove(item);
        }

        SavePackage();
    }

    /// <summary>升级物品</summary>
    public bool UpgradeItem(string uid)
    {
        var item = GetPackageItemByUid(uid);
        if (item == null) return false;

        var tableItem = GetTableItem(item.id);
        if (tableItem == null || tableItem.itemType == ItemType.Material)
        {
            Debug.Log(">>> 素材无法升级");
            return false;
        }

        if (item.level >= UIConst.MaxEquipmentLevel)
        {
            Debug.Log(">>> 已达到最大等级");
            return false;
        }

        item.level++;
        SavePackage();
        return true;
    }

    /// <summary>保存背包数据</summary>
    public void SavePackage()
    {
        PackageLocalData.Instance.SavePackage();
    }

    // ==================== 商店数据访问 ====================

    /// <summary>根据类型获取商店物品列表</summary>
    public List<StoreTableItem> GetStoreItemsByType(ItemType type)
    {
        var allItems = GameManager.Instance.GetStoreTable()?.DataList;
        if (allItems == null) return new List<StoreTableItem>();

        var result = new List<StoreTableItem>();
        foreach (var item in allItems)
        {
            if (item.itemType == type)
            {
                result.Add(item);
            }
        }
        return result;
    }

    /// <summary>获取商店物品配置</summary>
    public StoreTableItem GetStoreItem(int id)
    {
        return GameManager.Instance.GetStoreItemById(id);
    }
}
