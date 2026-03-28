using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// UI管理器 - 负责UI面板的加载、显示、隐藏
/// 功能：面板缓存、预制件加载、界面切换
/// </summary>
public class UIManager
{
    // ==================== 单例 ====================

    private static UIManager _instance;
    public static UIManager Instance => _instance ??= new UIManager();

    // ==================== 数据 ====================

    private Transform _uiRoot;                           // UI根节点
    private Dictionary<string, string> _pathDict;       // 面板路径配置
    private Dictionary<string, GameObject> _prefabDict; // 预制件缓存
    public Dictionary<string, BasePanel> panelDict;    // 已打开的面板

    // ==================== 构造函数 ====================

    private UIManager()
    {
        _prefabDict = new Dictionary<string, GameObject>();
        panelDict = new Dictionary<string, BasePanel>();
        _pathDict = new Dictionary<string, string>
        {
            { UIConst.PackagePanel, "PackagePanel" },
            { UIConst.StorePanel, "StorePanel" }
        };
    }

    // ==================== 属性 ====================

    /// <summary>获取UI根节点</summary>
    public Transform UIRoot
    {
        get
        {
            if (_uiRoot == null)
            {
                var canvas = GameObject.Find("Canvas");
                if (canvas != null)
                {
                    _uiRoot = canvas.transform;
                }
                else
                {
                    _uiRoot = new GameObject("Canvas").transform;
                }
            }
            return _uiRoot;
        }
    }

    // ==================== 面板操作 ====================

    /// <summary>获取已打开的面板</summary>
    public BasePanel GetPanel(string name)
    {
        if (panelDict.TryGetValue(name, out var panel))
        {
            return panel;
        }
        return null;
    }

    /// <summary>打开面板</summary>
    public BasePanel OpenPanel(string name)
    {
        // 检查是否已打开
        if (panelDict.TryGetValue(name, out var existingPanel))
        {
            Debug.Log("界面已打开: " + name);
            return existingPanel;
        }

        // 获取面板路径
        if (!_pathDict.TryGetValue(name, out var path))
        {
            Debug.Log("界面名称错误，或未配置路径: " + name);
            return null;
        }

        // 加载预制件
        GameObject panelPrefab = null;
        if (!_prefabDict.TryGetValue(name, out panelPrefab))
        {
            string realPath = UIConst.PrefabPath + path;
            panelPrefab = Resources.Load<GameObject>(realPath);

            if (panelPrefab == null)
            {
                Debug.LogError("加载面板失败: " + realPath);
                return null;
            }

            _prefabDict[name] = panelPrefab;
        }

        // 实例化面板
        GameObject panelObj = Object.Instantiate(panelPrefab, UIRoot, false);
        BasePanel panel = panelObj.GetComponent<BasePanel>();

        if (panel == null)
        {
            Debug.LogError("面板预制件缺少BasePanel组件: " + name);
            Object.Destroy(panelObj);
            return null;
        }

        // 注册并打开
        panelDict[name] = panel;
        panel.OpenPanel(name);

        return panel;
    }

    /// <summary>关闭面板</summary>
    public bool ClosePanel(string name)
    {
        if (!panelDict.TryGetValue(name, out var panel))
        {
            Debug.Log("界面未打开: " + name);
            return false;
        }

        panel.ClosePanel();
        return true;
    }
}
