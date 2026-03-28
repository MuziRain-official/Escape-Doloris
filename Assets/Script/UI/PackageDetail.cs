using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 物品详情面板
/// </summary>
public class PackageDetail : MonoBehaviour
{
    private Transform _icon;
    private Transform _name;
    private Transform _description;
    private Transform _number;

    private void Awake()
    {
        _icon = transform.Find("Center/icon");
        _name = transform.Find("Center/name");
        _description = transform.Find("Botton/description");
        _number = transform.Find("Botton/num");

        Clear();
    }

    public void Refresh(PackageLocalItem localItem)
    {
        var tableItem = ItemService.Instance.GetTableItem(localItem.id);
        if (tableItem == null)
        {
            Debug.LogError($"PackageDetail: 未找到物品配置, id={localItem.id}");
            return;
        }

        _name.GetComponent<Text>().text = tableItem.name;
        _description.GetComponent<Text>().text = tableItem.description;

        if (tableItem.itemType == ItemType.Material)
        {
            _number.GetComponent<Text>().text = $"持有数: {localItem.num}";
        }
        else
        {
            _number.GetComponent<Text>().text = $"等级: {localItem.level}";
        }

        var image = _icon?.GetComponent<Image>();
        if (image != null && tableItem.sprite != null)
        {
            image.sprite = tableItem.sprite;
            image.color = Color.white;
        }
    }

    public void Clear()
    {
        var image = _icon?.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = null;
            image.color = new Color(1, 1, 1, 0);
        }

        if (_name != null) _name.GetComponent<Text>().text = "";
        if (_description != null) _description.GetComponent<Text>().text = "";
        if (_number != null) _number.GetComponent<Text>().text = "";
    }
}
