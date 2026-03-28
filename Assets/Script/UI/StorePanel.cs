using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 商店面板
/// </summary>
public class StorePanel : BasePanel
{
    private Transform _materialBtn;
    private Transform _equipmentBtn;
    private Transform _scrollView;
    private Transform _buyBtn;
    private Transform _packageBtn;
    private GameObject _itemPrefab;

    private ItemType _currentType = ItemType.Material;
    private StoreTableItem _selectedItem;
    private StoreCell _currentSelectedCell;

    protected new void Awake()
    {
        base.Awake();
        InitUI();
    }

    private void Start()
    {
        EventSystem.Instance.OnItemBought += OnItemBoughtHandler;
        RefreshUI();
    }

    private void OnDestroy()
    {
        EventSystem.Instance.OnItemBought -= OnItemBoughtHandler;
    }

    private void InitUI()
    {
        _materialBtn = transform.Find("Top/Material");
        _equipmentBtn = transform.Find("Top/Equipment");
        _scrollView = transform.Find("Center/Scroll View");
        _buyBtn = transform.Find("Center/Buy");
        _packageBtn = transform.Find("Bottom/Package");

        Transform itemPrefabTrans = transform.Find("Center/Scroll View/Viewport/Content/StoreUIItem");
        if (itemPrefabTrans != null)
        {
            _itemPrefab = itemPrefabTrans.gameObject;
            _itemPrefab.SetActive(false);
        }

        _materialBtn.GetComponent<Button>().onClick.AddListener(OnClickMaterial);
        _equipmentBtn.GetComponent<Button>().onClick.AddListener(OnClickEquipment);
        _buyBtn.GetComponent<Button>().onClick.AddListener(OnClickBuy);
        _packageBtn.GetComponent<Button>().onClick.AddListener(OnClickPackage);
    }

    private void OnItemBoughtHandler(int itemId)
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        RefreshScroll();
    }

    private void RefreshScroll()
    {
        RectTransform scrollContent = _scrollView.GetComponent<ScrollRect>().content;

        for (int i = scrollContent.childCount - 1; i >= 0; i--)
        {
            Transform child = scrollContent.GetChild(i);
            if (child.gameObject != _itemPrefab && child.gameObject.activeSelf)
            {
                Destroy(child.gameObject);
            }
        }

        var storeItems = ItemService.Instance.GetStoreItemsByType(_currentType);
        foreach (var storeItem in storeItems)
        {
            GameObject cellObj = Instantiate(_itemPrefab, scrollContent);
            cellObj.SetActive(true);

            StoreCell cell = cellObj.GetComponent<StoreCell>();
            cell.Refresh(storeItem, OnStoreCellClicked);
        }
    }

    private void OnStoreCellClicked(StoreTableItem item, StoreCell cell)
    {
        if (_currentSelectedCell != null)
        {
            _currentSelectedCell.SetSelected(false);
        }

        _selectedItem = item;
        _currentSelectedCell = cell;
        _currentSelectedCell.SetSelected(true);
    }

    private void OnClickMaterial()
    {
        _currentType = ItemType.Material;
        ClearSelection();
        RefreshScroll();
    }

    private void OnClickEquipment()
    {
        _currentType = ItemType.Equipment;
        ClearSelection();
        RefreshScroll();
    }

    private void ClearSelection()
    {
        if (_currentSelectedCell != null)
        {
            _currentSelectedCell.SetSelected(false);
            _currentSelectedCell = null;
            _selectedItem = null;
        }
    }

    private void OnClickBuy()
    {
        if (_selectedItem == null)
        {
            Debug.Log(">>> 请先选择要购买的商品");
            return;
        }

        ItemService.Instance.AddItem(_selectedItem.id, 1);
        EventSystem.Instance.EmitItemBought(_selectedItem.id);
        Debug.Log($">>> 购买成功: {_selectedItem.name}");
    }

    private void OnClickPackage()
    {
        UIManager.Instance.OpenPanel(UIConst.PackagePanel);
    }
}
