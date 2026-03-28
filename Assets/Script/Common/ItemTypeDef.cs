using UnityEngine;

/// <summary>
/// 物品类型枚举 - 统一所有物品类型定义
/// </summary>
public enum ItemType
{
    Equipment,  // 装备：独立实例，支持升级
    Material   // 素材：可合并显示，无等级
}

/// <summary>
/// 常量定义类 - 集中管理所有字符串常量
/// </summary>
public static class UIConst
{
    // 面板名称
    public const string PackagePanel = "PackagePanel";
    public const string StorePanel = "StorePanel";

    // 文件路径
    public const string PackageTablePath = "Data/PackageTable";
    public const string StoreTablePath = "Data/StoreTable";
    public const string PrefabPath = "Prefab/";

    // 游戏配置
    public const int MaxEquipmentLevel = 3;  // 装备最大等级
}

/// <summary>
/// 通用回调定义 - 用于事件系统
/// </summary>
public delegate void VoidCallback();
public delegate void ItemCallback<T>(T param);
