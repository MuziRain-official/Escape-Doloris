using System;
using System.Collections.Generic;

/// <summary>
/// 事件系统 - 简单的事件中心，用于解耦UI与业务逻辑
/// 使用方法：
/// 1. 订阅：EventSystem.Instance.On[事件名] += 处理方法;
/// 2. 发布：EventSystem.Instance.Emit[事件名](参数);
/// 3. 取消订阅：EventSystem.Instance.On[事件名] -= 处理方法;
/// </summary>
public class EventSystem
{
    private static EventSystem _instance;
    public static EventSystem Instance => _instance ??= new EventSystem();

    // 事件字典
    private Dictionary<string, Delegate> _events = new Dictionary<string, Delegate>();

    private EventSystem() { }

    // ==================== 背包相关事件 ====================

    // 背包数据更新时触发
    public event VoidCallback OnPackageRefresh
    {
        add => AddEventListener("PackageRefresh", value);
        remove => RemoveEventListener("PackageRefresh", value);
    }
    public void EmitPackageRefresh() => Emit("PackageRefresh");

    // 物品被选中时触发 (参数: 物品UID)
    public event ItemCallback<string> OnItemSelected
    {
        add => AddEventListener("ItemSelected", value);
        remove => RemoveEventListener("ItemSelected", value);
    }
    public void EmitItemSelected(string uid) => Emit("ItemSelected", uid);

    // 物品被购买时触发 (参数: 物品ID)
    public event ItemCallback<int> OnItemBought
    {
        add => AddEventListener("ItemBought", value);
        remove => RemoveEventListener("ItemBought", value);
    }
    public void EmitItemBought(int itemId) => Emit("ItemBought", itemId);

    // 物品被出售时触发 (参数: 物品UID)
    public event ItemCallback<string> OnItemSold
    {
        add => AddEventListener("ItemSold", value);
        remove => RemoveEventListener("ItemSold", value);
    }
    public void EmitItemSold(string uid) => Emit("ItemSold", uid);

    // 物品升级时触发 (参数: 物品UID)
    public event ItemCallback<string> OnItemUpgraded
    {
        add => AddEventListener("ItemUpgraded", value);
        remove => RemoveEventListener("ItemUpgraded", value);
    }
    public void EmitItemUpgraded(string uid) => Emit("ItemUpgraded", uid);

    // ==================== 内部方法 ====================

    private void AddEventListener(string eventName, Delegate callback)
    {
        if (!_events.ContainsKey(eventName))
        {
            _events[eventName] = null;
        }
        _events[eventName] = Delegate.Combine(_events[eventName], callback);
    }

    private void RemoveEventListener(string eventName, Delegate callback)
    {
        if (_events.ContainsKey(eventName))
        {
            _events[eventName] = Delegate.Remove(_events[eventName], callback);
        }
    }

    private void Emit(string eventName)
    {
        if (_events.TryGetValue(eventName, out var callback))
        {
            (callback as VoidCallback)?.Invoke();
        }
    }

    private void Emit<T>(string eventName, T param)
    {
        if (_events.TryGetValue(eventName, out var callback))
        {
            (callback as ItemCallback<T>)?.Invoke(param);
        }
    }
}
