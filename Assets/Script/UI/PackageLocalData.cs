using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

/// <summary>
/// 背包本地数据管理 - 负责背包数据的持久化存储
/// 存储位置：Application.persistentDataPath/PackageData.json
/// </summary>
public class PackageLocalData
{
    private static PackageLocalData _instance;

    public static PackageLocalData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PackageLocalData();
            }
            return _instance;
        }
    }

    /// <summary>背包物品列表</summary>
    public List<PackageLocalItem> items;

    private string GetFilePath()
    {
        return Application.persistentDataPath + "/PackageData.json";
    }

    /// <summary>保存背包数据到本地</summary>
    public void SavePackage()
    {
        string directory = Application.persistentDataPath;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        string inventoryJson = JsonConvert.SerializeObject(this);
        File.WriteAllText(GetFilePath(), inventoryJson);
    }

    /// <summary>从本地加载背包数据</summary>
    public List<PackageLocalItem> LoadPackage()
    {
        // 如果已有缓存，直接返回
        if (items != null)
        {
            return items;
        }

        string path = GetFilePath();
        if (File.Exists(path))
        {
            string inventoryJson = File.ReadAllText(path);
            PackageLocalData packageLocalData = JsonConvert.DeserializeObject<PackageLocalData>(inventoryJson);
            items = packageLocalData?.items ?? new List<PackageLocalItem>();
            return items;
        }
        else
        {
            items = new List<PackageLocalItem>();
            return items;
        }
    }
}

/// <summary>
/// 背包物品实例数据 - 运行时动态数据
/// </summary>
[System.Serializable]
public class PackageLocalItem
{
    /// <summary>唯一标识符</summary>
    public string uid;

    /// <summary>物品配置ID</summary>
    public int id;

    /// <summary>物品数量（素材使用）</summary>
    public int num = 3;

    /// <summary>物品等级（装备使用）</summary>
    public int level = 1;

    public override string ToString()
    {
        return string.Format("[id]:{0} [num]:{1} [level]:{2}", id, num, level);
    }
}