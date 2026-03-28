using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// UI基础组件 - 提供常用的UI初始化和查找方法
/// 目的：减少重复代码，统一UI组件的初始化方式
/// </summary>
public class UIBase : MonoBehaviour
{
    // ==================== 组件缓存 ====================

    /// <summary>缓存Transform组件</summary>
    protected Transform CachedTransform { get; private set; }

    /// <summary>缓存RectTransform组件</summary>
    protected RectTransform RectTransform { get; private set; }

    /// <summary>缓存GameObject</summary>
    protected GameObject CachedGameObject { get; private set; }

    protected virtual void Awake()
    {
        CachedTransform = transform;
        RectTransform = GetComponent<RectTransform>();
        CachedGameObject = gameObject;
        OnAwake();
    }

    /// <summary>子类可重写的Awake逻辑</summary>
    protected virtual void OnAwake() { }

    // ==================== 组件查找便捷方法 ====================

    /// <summary>通过路径查找子物体Transform</summary>
    protected Transform FindChild(string path)
    {
        return transform.Find(path);
    }

    /// <summary>通过路径查找子物体并获取组件</summary>
    protected T FindComponent<T>(string path) where T : Component
    {
        var trans = transform.Find(path);
        return trans != null ? trans.GetComponent<T>() : null;
    }

    /// <summary>获取自身组件的便捷方法</summary>
    protected T GetMyComponent<T>() where T : Component
    {
        return GetComponent<T>();
    }

    // ==================== 按钮绑定便捷方法 ====================

    /// <summary>绑定按钮点击事件</summary>
    protected void BindButton(string path, VoidCallback callback)
    {
        var btn = FindComponent<Button>(path);
        if (btn != null)
        {
            btn.onClick.AddListener(() => callback?.Invoke());
        }
        else
        {
            Debug.LogWarning($"[UIBase] Button not found: {path}");
        }
    }

    /// <summary>绑定多个按钮</summary>
    protected void BindButtons(VoidCallback[] callbacks, params string[] paths)
    {
        for (int i = 0; i < paths.Length && i < callbacks.Length; i++)
        {
            BindButton(paths[i], callbacks[i]);
        }
    }

    // ==================== 物品数据便捷方法 ====================

    /// <summary>获取物品静态配置（通过ItemService）</summary>
    protected PackageTableItem GetTableItem(int id)
    {
        return ItemService.Instance.GetTableItem(id);
    }

    /// <summary>获取物品图标精灵</summary>
    protected Sprite GetItemSprite(int id)
    {
        var item = GetTableItem(id);
        return item?.sprite;
    }

    // ==================== 通用UI操作 ====================

    /// <summary>设置物体激活状态</summary>
    protected void SetActive(GameObject obj, bool active)
    {
        if (obj != null) obj.SetActive(active);
    }

    /// <summary>设置物体激活状态</summary>
    protected void SetActive(Transform trans, bool active)
    {
        if (trans != null) trans.gameObject.SetActive(active);
    }

    /// <summary>设置Text文本</summary>
    protected void SetText(Transform textTrans, string content)
    {
        if (textTrans != null)
        {
            var text = textTrans.GetComponent<Text>();
            if (text != null) text.text = content;
        }
    }

    /// <summary>设置Image图片</summary>
    protected void SetImageSprite(Transform imageTrans, Sprite sprite)
    {
        if (imageTrans != null)
        {
            var img = imageTrans.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = sprite;
                img.color = sprite != null ? Color.white : new Color(1, 1, 1, 0);
            }
        }
    }
}

/// <summary>
/// 物品格子基类 - 提供物品Cell的通用功能
/// </summary>
public abstract class ItemCellBase : UIBase, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    // 当前物品数据
    protected PackageLocalItem _localItem;
    protected PackageTableItem _tableItem;

    // 当前选中的UID（静态变量，所有Cell共享）
    protected static string _selectedUid;

    // 选中高亮
    [SerializeField] protected Transform _selectMark;

    // 点击事件回调
    protected VoidCallback onClicked;
    protected VoidCallback onSelected;

    protected override void OnAwake()
    {
        base.OnAwake();
        InitCell();
    }

    /// <summary>子类实现：初始化格子特定的UI元素</summary>
    protected abstract void InitCell();

    /// <summary>刷新格子显示</summary>
    public void Refresh(PackageLocalItem localItem, VoidCallback clickedCallback = null, VoidCallback selectedCallback = null)
    {
        _localItem = localItem;
        _tableItem = ItemService.Instance.GetTableItem(localItem.id);
        onClicked = clickedCallback;
        onSelected = selectedCallback;

        UpdateDisplay();
        UpdateSelectionState();
    }

    /// <summary>子类实现：更新显示内容</summary>
    protected abstract void UpdateDisplay();

    /// <summary>更新选中状态显示</summary>
    protected void UpdateSelectionState()
    {
        bool isSelected = _localItem != null && _localItem.uid == _selectedUid;
        if (_selectMark != null)
        {
            _selectMark.gameObject.SetActive(isSelected);
        }
    }

    /// <summary>设置当前选中的UID</summary>
    public static void SetSelectedUid(string uid)
    {
        _selectedUid = uid;
        // 注意：这里通过事件通知刷新，不需要手动刷新所有Cell
    }

    // ==================== 鼠标事件接口实现 ====================

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_localItem == null) return;

        // 触发点击回调
        onClicked?.Invoke();

        // 选中新物品
        _selectedUid = _localItem.uid;
        UpdateSelectionState();

        // 触发选中回调
        onSelected?.Invoke();

        // 发出选中事件
        EventSystem.Instance.EmitItemSelected(_localItem.uid);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 鼠标悬停时显示选中标记
        if (_selectMark != null && _localItem != null && _localItem.uid != _selectedUid)
        {
            _selectMark.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 鼠标离开时恢复选中状态
        UpdateSelectionState();
    }
}
