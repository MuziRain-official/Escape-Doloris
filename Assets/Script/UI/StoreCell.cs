using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 商店物品格子 - 显示商店中的单个商品
/// </summary>
public class StoreCell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    // ==================== 组件 ====================

    [Header("UI组件")]
    [SerializeField] private Transform _icon;
    [SerializeField] private Transform _selectMark;

    // ==================== 数据 ====================

    private StoreTableItem _storeItem;
    private bool _isSelected = false;

    // 点击回调
    private System.Action<StoreTableItem, StoreCell> _onClicked;

    // ==================== 生命周期 ====================

    private void Awake()
    {
        // 查找UI组件
        _icon = transform.Find("icon");
        _selectMark = transform.Find("select");

        // 默认隐藏选中标记
        if (_selectMark != null)
            _selectMark.gameObject.SetActive(false);
    }

    // ==================== 公共方法 ====================

    /// <summary>刷新格子显示</summary>
    public void Refresh(StoreTableItem storeItem, System.Action<StoreTableItem, StoreCell> onClicked = null)
    {
        _storeItem = storeItem;
        _onClicked = onClicked;

        // 显示图标
        if (_icon != null)
        {
            var image = _icon.GetComponent<Image>();
            if (image != null && storeItem.sprite != null)
            {
                image.sprite = storeItem.sprite;
            }
        }
    }

    /// <summary>设置选中状态</summary>
    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        if (_selectMark != null)
        {
            _selectMark.gameObject.SetActive(selected);
        }
    }

    // ==================== 鼠标事件 ====================

    public void OnPointerClick(PointerEventData eventData)
    {
        // 触发点击回调
        _onClicked?.Invoke(_storeItem, this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 悬停时显示选中标记（如果未选中）
        if (!_isSelected && _selectMark != null)
        {
            _selectMark.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 离开时恢复选中状态
        if (!_isSelected && _selectMark != null)
        {
            _selectMark.gameObject.SetActive(false);
        }
    }
}
