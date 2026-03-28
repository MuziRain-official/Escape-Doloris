using UnityEngine;

/// <summary>
/// UI面板基类
/// </summary>
public class BasePanel : MonoBehaviour
{
    protected bool isRemove = false;
    protected string panelName;

    protected virtual void Awake()
    {
    }

    public virtual void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public virtual void OpenPanel(string name)
    {
        panelName = name;
        SetActive(true);
    }

    public virtual void ClosePanel()
    {
        isRemove = true;
        SetActive(false);

        if (UIManager.Instance.panelDict.ContainsKey(panelName))
        {
            UIManager.Instance.panelDict.Remove(panelName);
        }

        Destroy(gameObject);
    }
}
