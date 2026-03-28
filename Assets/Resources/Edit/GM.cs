using UnityEngine;
using UnityEditor;

public class GM
{
    [MenuItem("CM/SaveLocalConfig")]
    public static void SaveLocalConfig()
    {
        Debug.Log(Application.persistentDataPath);
        for (int i=0; i<5; i++)
        {
            UserData userData = new UserData();
            userData.name = "xiaoqi" + i.ToString();
            userData.level = i;
            LocalConfig.SaveUserData(userData);
        }
        Debug.Log("Save End !!!!!!!!!!!!!!!!!!!!");
    }

    [MenuItem("CM/LoadLocalConfig")]
    public static void LoadLocalConfig()
    {
        for (int i = 0; i < 5; i++)
        {
            string name = "xiaoqi" + i.ToString();
            UserData userData = LocalConfig.LoadUserData(name);
            Debug.Log(userData.name);
            Debug.Log(userData.level);
        }
    }

}