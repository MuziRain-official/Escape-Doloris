using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 背包面板 - 显示玩家背包中的物品
/// </summary>
public class PackagePanel : BasePanel
{
    // UI组件（通过代码自动查找）
    private Transform _closeBtn;
    private Transform _scrollView;
    private Transform _detailPanel;
    private Transform _sellBtn;
    private Transform _upgradeBtn;
    private GameObject _packageItemPrefab;

    private string _selectedUid;
    private PackageCell _currentSelectedCell;

    // ==================== 生命周期 ====================

    protected new void Awake()
    {
        base.Awake();
        InitUI();
    }

    private void Start()
    {
        EventSystem.Instance.OnPackageRefresh += OnPackageRefreshHandler;
        EventSystem.Instance.OnItemSelected += OnItemSelectedHandler;
        RefreshUI();
    }

    private void OnDestroy()
    {
        EventSystem.Instance.OnPackageRefresh -= OnPackageRefreshHandler;
        EventSystem.Instance.OnItemSelected -= OnItemSelectedHandler;
    }

    // ==================== 初始化 ====================

    private void InitUI()
    {
        _closeBtn = transform.Find("Top/close");
        _scrollView = transform.Find("Center/Scroll View");
        _detailPanel = transform.Find("Center/DetailPanel");
        _sellBtn = transform.Find("Bottom/sell");
        _upgradeBtn = transform.Find("Bottom/upgrade");

        // 绑定按钮事件
        _closeBtn.GetComponent<Button>().onClick.AddListener(OnClickClose);
        _sellBtn.GetComponent<Button>().onClick.AddListener(OnClickSell);
        _upgradeBtn.GetComponent<Button>().onClick.AddListener(OnClickUpgrade);
    }

    // ==================== 事件处理 ====================

    private void OnPackageRefreshHandler()
    {
        RefreshUI();
    }

    private void OnItemSelectedHandler(string uid)
    {
        _selectedUid = uid;
        RefreshDetail();
    }

    // ==================== 刷新显示 ====================

    private void RefreshUI()
    {
        RefreshScroll();
        RefreshDetail();
    }

    private void RefreshScroll()
    {
        // 获取预制件（从Resources加载）
        if (_packageItemPrefab == null)
            _packageItemPrefab = Resources.Load<GameObject>("Prefab/PackUIItem");

        RectTransform scrollContent = _scrollView.GetComponent<ScrollRect>().content;

        for (int i = scrollContent.childCount - 1; i >= 0; i--)
        {
            Destroy(scrollContent.GetChild(i).gameObject);
        }

        _currentSelectedCell = null;

        var packageData = ItemService.Instance.GetProcessedPackageData();
        foreach (var localItem in packageData)
        {
            GameObject cellObj = Instantiate(_packageItemPrefab, scrollContent);
            PackageCell cell = cellObj.GetComponent<PackageCell>();
            cell.Refresh(localItem, () =>
            {
                // 取消之前选中的
                if (_currentSelectedCell != null)
                {
                    _currentSelectedCell.SetSelected(false);
                }
                // 选中当前
                _currentSelectedCell = cell;
                _currentSelectedCell.SetSelected(true);

                _selectedUid = localItem.uid;
                RefreshDetail();
            });

            // 恢复选中状态
            if (localItem.uid == _selectedUid)
            {
                _currentSelectedCell = cell;
                cell.SetSelected(true);
            }
        }
    }

    private void RefreshDetail()
    {
        var detail = _detailPanel.GetComponent<PackageDetail>();

        if (string.IsNullOrEmpty(_selectedUid))
        {
            detail.Clear();
            return;
        }

        var localItem = ItemService.Instance.GetPackageItemByUid(_selectedUid);
        if (localItem == null)
        {
            detail.Clear();
            return;
        }

        detail.Refresh(localItem);
    }

    // ==================== 按钮点击 ====================

    private void OnClickClose()
    {
        ClosePanel();
    }

    private void OnClickSell()
    {
        if (string.IsNullOrEmpty(_selectedUid))
        {
            Debug.Log(">>> 请先选择要出售的物品");
            return;
        }

        var item = ItemService.Instance.GetPackageItemByUid(_selectedUid);
        if (item == null)
        {
            Debug.Log(">>> 物品不存在");
            return;
        }

        ItemService.Instance.RemoveItem(_selectedUid, 1);

        var packageData = ItemService.Instance.GetProcessedPackageData();
        var exists = packageData.Exists(x => x.id == item.id && x.uid == item.uid);

        if (!exists)
        {
            _selectedUid = null;
        }

        EventSystem.Instance.EmitPackageRefresh();
        Debug.Log(">>> 出售成功");
    }

    private void OnClickUpgrade()
    {
        if (string.IsNullOrEmpty(_selectedUid))
        {
            Debug.Log(">>> 请先选择要升级的物品");
            return;
        }

        var item = ItemService.Instance.GetPackageItemByUid(_selectedUid);
        if (item == null)
        {
            Debug.Log(">>> 物品不存在");
            return;
        }

        var tableItem = ItemService.Instance.GetTableItem(item.id);
        if (tableItem == null || tableItem.itemType == ItemType.Material)
        {
            Debug.Log(">>> 素材无法升级");
            return;
        }

        bool success = ItemService.Instance.UpgradeItem(_selectedUid);
        if (success)
        {
            RefreshDetail();
            EventSystem.Instance.EmitPackageRefresh();
            Debug.Log(">>> 升级成功");
        }
    }
}
