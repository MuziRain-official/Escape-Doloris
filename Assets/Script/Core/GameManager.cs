using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 游戏管理器 - 负责游戏的核心数据管理
/// 职责：管理背包数据、商店数据、系统配置
/// </summary>
public class GameManager : MonoBehaviour
{
    // ==================== 单例 ====================

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("GameManager");
                _instance = go.AddComponent<GameManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    // ==================== 数据 ====================

    private PackageTable _packageTable;
    private StoreTable _storeTable;

    // ==================== 生命周期 ====================

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 默认打开背包面板
        UIManager.Instance.OpenPanel(UIConst.PackagePanel);
    }

    // ==================== 背包数据访问 ====================

    /// <summary>获取背包配置表</summary>
    public PackageTable GetPackageTable()
    {
        if (_packageTable == null)
        {
            _packageTable = Resources.Load<PackageTable>(UIConst.PackageTablePath);
        }
        return _packageTable;
    }

    /// <summary>获取背包本地数据</summary>
    public List<PackageLocalItem> GetPackageLocalData()
    {
        return PackageLocalData.Instance.LoadPackage();
    }

    /// <summary>根据ID获取物品配置</summary>
    public PackageTableItem GetPackageItemById(int id)
    {
        var list = GetPackageTable()?.DataList;
        if (list == null) return null;

        foreach (var item in list)
        {
            if (item.id == id) return item;
        }
        return null;
    }

    /// <summary>根据UID获取物品实例</summary>
    public PackageLocalItem GetPackageLocalItemByUId(string uid)
    {
        var list = GetPackageLocalData();
        foreach (var item in list)
        {
            if (item.uid == uid) return item;
        }
        return null;
    }

    // ==================== 商店数据访问 ====================

    /// <summary>获取商店配置表</summary>
    public StoreTable GetStoreTable()
    {
        if (_storeTable == null)
        {
            _storeTable = Resources.Load<StoreTable>(UIConst.StoreTablePath);
        }
        return _storeTable;
    }

    /// <summary>根据ID获取商店物品配置</summary>
    public StoreTableItem GetStoreItemById(int id)
    {
        var list = GetStoreTable()?.DataList;
        if (list == null) return null;

        foreach (var item in list)
        {
            if (item.id == id) return item;
        }
        return null;
    }

    // ==================== 背包操作 ====================

    /// <summary>获取排序后的背包数据</summary>
    public List<PackageLocalItem> GetSortPackageLocalData()
    {
        var list = PackageLocalData.Instance.LoadPackage();
        list.Sort(new PackageItemComparer());
        return list;
    }

    /// <summary>添加物品到背包</summary>
    public void AddItemToPackage(int itemId, int count)
    {
        var packageData = GetPackageLocalData();
        var tableItem = GetPackageItemById(itemId);

        if (tableItem == null)
        {
            Debug.LogWarning("商品不存在: " + itemId);
            return;
        }

        if (tableItem.itemType == ItemType.Material)
        {
            // 素材：查找相同ID，有则累加
            var existing = packageData.Find(x => x.id == itemId);
            if (existing != null)
            {
                existing.num += count;
            }
            else
            {
                CreateNewItem(packageData, itemId, count, 1);
            }
        }
        else // Equipment
        {
            // 装备：每个实例单独添加
            for (int i = 0; i < count; i++)
            {
                CreateNewItem(packageData, itemId, 1, 1);
            }
        }

        PackageLocalData.Instance.SavePackage();
        Debug.Log("已添加到背包: " + tableItem.name + " x" + count);
    }

    private void CreateNewItem(List<PackageLocalItem> packageData, int id, int num, int level)
    {
        var newItem = new PackageLocalItem
        {
            uid = System.Guid.NewGuid().ToString(),
            id = id,
            num = num,
            level = level
        };
        packageData.Add(newItem);
    }

    /// <summary>获取处理后的物品列表（装备单独显示，素材合并显示）</summary>
    public List<PackageLocalItem> GetProcessedPackageData()
    {
        var rawItems = PackageLocalData.Instance.LoadPackage();
        var result = new List<PackageLocalItem>();

        // 先排序
        rawItems.Sort(new PackageItemComparer());

        foreach (var item in rawItems)
        {
            var tableItem = GetPackageItemById(item.id);
            if (tableItem == null) continue;

            if (tableItem.itemType == ItemType.Equipment)
            {
                // 装备：直接添加
                result.Add(item);
            }
            else // Material
            {
                // 素材：合并相同ID
                var existing = result.Find(x => x.id == item.id);
                if (existing != null)
                {
                    existing.num += item.num;
                }
                else
                {
                    result.Add(new PackageLocalItem
                    {
                        uid = item.uid,
                        id = item.id,
                        num = item.num,
                        level = 0
                    });
                }
            }
        }

        return result;
    }
}

/// <summary>
/// 背包物品比较器 - 按ID排序
/// </summary>
public class PackageItemComparer : IComparer<PackageLocalItem>
{
    public int Compare(PackageLocalItem a, PackageLocalItem b)
    {
        return b.id.CompareTo(a.id);
    }
}
