# 背包系统架构说明

## 一、目录结构

```
Assets/Script/
├── Common/                      # 公共定义
│   └── ItemTypeDef.cs           # 枚举和常量定义
│
├── Core/                        # 核心层 - 业务逻辑
│   ├── GameManager.cs           # 游戏管理器
│   ├── ItemService.cs           # 物品服务层
│   └── EventSystem.cs           # 事件系统
│
├── UI/                          # UI层 - 界面显示
│   ├── Common/
│   │   └── UIBase.cs            # UI基类组件
│   │
│   ├── BasePanel.cs             # 面板基类
│   ├── UIManager.cs             # UI管理器
│   │
│   ├── PackagePanel.cs          # 背包界面
│   ├── PackageCell.cs           # 背包物品格子
│   ├── PackageDetail.cs         # 物品详情
│   ├── PackageTable.cs          # 背包配置表
│   └── PackageLocalData.cs      # 背包本地数据
│
│   ├── StorePanel.cs            # 商店界面
│   ├── StoreCell.cs            # 商店物品格子
│   └── StoreTable.cs           # 商店配置表
```

---

## 二、分层架构

### 1. Common（公共定义层）
- **ItemTypeDef.cs**: 定义枚举和常量
  - `ItemType`: 物品类型（装备/素材）
  - `UIConst`: UI相关常量
  - 回调delegate定义

### 2. Core（核心业务层）
负责数据管理和业务逻辑，与UI解耦

| 类 | 职责 |
|---|---|
| **GameManager** | 游戏主管理器，管理所有数据 |
| **ItemService** | 物品服务，封装数据操作 |
| **EventSystem** | 事件中心，解耦UI与业务 |

### 3. UI（界面层）
负责界面显示和用户交互

| 类 | 职责 |
|---|---|
| **UIBase** | UI组件基类，提供便捷方法 |
| **BasePanel** | 面板基类，管理面板生命周期 |
| **UIManager** | UI管理，加载和显示面板 |

---

## 三、数据流

```
用户点击
    ↓
UI层 (PackagePanel/StorePanel)
    ↓
事件系统 (EventSystem)
    ↓
业务层 (ItemService → GameManager)
    ↓
数据层 (PackageLocalData)
    ↓
保存到本地
    ↓
发出事件通知UI刷新
    ↓
UI层响应事件，刷新显示
```

---

## 四、使用示例

### 1. 添加物品到背包
```csharp
// 通过服务层添加
ItemService.Instance.AddItem(itemId, count);

// 或者通过事件通知
EventSystem.Instance.EmitItemBought(itemId);
```

### 2. 订阅背包刷新事件
```csharp
// 在 Start 中订阅
EventSystem.Instance.OnPackageRefresh += OnRefreshHandler;

// 在 OnDestroy 中取消订阅
EventSystem.Instance.OnPackageRefresh -= OnRefreshHandler;
```

### 3. 创建新的UI面板
```csharp
public class MyPanel : BasePanel
{
    protected override void OnAwake()
    {
        // 使用便捷方法查找UI
        var btn = FindComponent<Button>("ButtonPath");
        BindButton("ButtonPath", OnClickHandler);
    }
}
```

---

## 五、架构原则

1. **依赖倒置**: UI层通过ItemService访问数据，不直接调用GameManager
2. **事件驱动**: 使用EventSystem解耦UI与业务
3. **单一职责**: 每个类只负责一件事
4. **开闭原则**: 通过事件扩展功能，不修改已有代码
