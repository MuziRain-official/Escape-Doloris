using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 背包物品格子
/// </summary>
public class PackageCell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Transform _icon;
    private Transform _level;
    private Transform _lv2;
    private Transform _lv3;
    private Transform _selectMark;

    private PackageLocalItem _localItem;
    private PackageTableItem _tableItem;
    private System.Action _onClicked;
    private bool _isSelected = false;

    private void Awake()
    {
        _icon = transform.Find("icon");
        _level = transform.Find("level");
        _lv2 = transform.Find("bk2");
        _lv3 = transform.Find("bk3");
        _selectMark = transform.Find("select");

        if (_selectMark != null)
            _selectMark.gameObject.SetActive(false);
    }

    public void Refresh(PackageLocalItem localItem, System.Action clickedCallback = null)
    {
        _localItem = localItem;
        _tableItem = ItemService.Instance.GetTableItem(localItem.id);
        _onClicked = clickedCallback;

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (_localItem == null || _tableItem == null) return;

        // 图标
        var image = _icon?.GetComponent<Image>();
        if (image != null && _tableItem.sprite != null)
        {
            image.sprite = _tableItem.sprite;
            image.color = Color.white;
        }

        // 根据物品类型显示
        if (_tableItem.itemType == ItemType.Material)
        {
            _level.GetComponent<Text>().text = $"x{_localItem.num}";
            _lv2.gameObject.SetActive(false);
            _lv3.gameObject.SetActive(false);
        }
        else
        {
            _level.GetComponent<Text>().text = $"Lv.{_localItem.level}";
            _lv2.gameObject.SetActive(_localItem.level == 2);
            _lv3.gameObject.SetActive(_localItem.level == 3);
        }
    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        if (_selectMark != null)
        {
            _selectMark.gameObject.SetActive(selected);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _onClicked?.Invoke();
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
