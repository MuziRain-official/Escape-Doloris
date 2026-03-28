// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEditor;
// using UnityEngine;

// public class GMCmd
// {
//     [MenuItem("CMCmd/读取表格")]
//     public static void ReadTable()
//     {
//         PackageTable packageTable = Resources.Load<PackageTable>("Data/PackageTable");
//         foreach (PackageTableItem packageItem in packageTable.DataList)
//         {
//             Debug.Log(string.Format("【id】:{0}, 【name】:{1}", packageItem.id, packageItem.name));
//         }
//     }

//     [MenuItem("CMCmd/创建背包测试数据")]
//     public static void CreateLocalPackageData()
//     {
//         // 保存数据
//         PackageLocalData.Instance.items = new List<PackageLocalItem>();

//         // 装备 (id 1-5)：每个创建2-3个独立实例，有不同等级
//         for (int id = 1; id <= 5; id++)
//         {
//             for (int j = 0; j < 3; j++)
//             {
//                 PackageLocalItem packageLocalItem = new()
//                 {
//                     uid = Guid.NewGuid().ToString(),
//                     id = id,
//                     num = 1,
//                     level = j + 1,  // 等级1-3
//                 };
//                 PackageLocalData.Instance.items.Add(packageLocalItem);
//             }
//         }

//         // 素材 (id 6-8)：每个创建多个，数量合并显示
//         for (int id = 6; id <= 8; id++)
//         {
//             PackageLocalItem packageLocalItem = new()
//             {
//                 uid = Guid.NewGuid().ToString(),
//                 id = id,
//                 num = 5,
//                 level = 0,
//             };
//             PackageLocalData.Instance.items.Add(packageLocalItem);
//         }

//         PackageLocalData.Instance.SavePackage();
//     }

//     [MenuItem("CMCmd/读取背包测试数据")]
//     public static void ReadLocalPackageData()
//     {
//         // 读取数据
//         List<PackageLocalItem> readItems = PackageLocalData.Instance.LoadPackage();
//         foreach (PackageLocalItem item in readItems)
//         {
//             Debug.Log(item);
//         }
//     }

//     [MenuItem("CMCmd/打开背包主界面")]
//     public static void OpenPackagePanel()
//     {
//         UIManager.Instance.OpenPanel(UIConst.PackagePanel);
//     }
// }
