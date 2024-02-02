using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Page : MonoBehaviour
{
    public Condition[] conditions;

    [SerializeField]
    private UnityEvent pageReloaded;

    public void Reload()
    {
        OnReloaded();
        pageReloaded?.Invoke();
    }

    protected virtual void OnReloaded() { }

    public void GotoPage(Page page)
    {
        Encyclopedia.Show(page);
    }

    public void Hide()
    {
        Encyclopedia.Hide();
    }

    public bool Validate()
    {
        if(conditions != null && conditions.Length > 0)
        {
            foreach (var condition in conditions)
            {
                if (!condition.Validate())
                    return false;
            }
        }        

        return true;
    }
}
