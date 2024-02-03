using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Page : MonoBehaviour
{
    [System.Serializable]
    public class ConditionalText
    {
        public Condition condition;
        [TextArea] public string text;        
    }

    public Condition[] conditions;

    public virtual void ReloadData() { }

    public void GotoPage(int page)
    {
        Encyclopedia.Show(page);
    }

    public void GotoChapter(string chapter)
    {
        Encyclopedia.Show<PageChapter>(chapter, 1);
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

    public virtual bool Search(string text)
    {
        return false;
    }
}
