using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encyclopedia : MonoBehaviour
{
    private static Encyclopedia instance;

    public Book book;
    public AutoFlip autoFlip;
    public UIAnimator animator;
    public AnimationClip show;
    public AnimationClip hide;

    [Header("Pages")]
    public Page frontPage;
    public Page indexPage;
    public List<Page> pages;
    public Page emptyPage;
    public Page backPage;

    private List<Page> actualPages;
    private static bool isVisible;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        actualPages = new List<Page>();
        actualPages.Add(frontPage);
        actualPages.Add(indexPage);
        
        foreach (var page in pages)
        {
            if(page.Validate())
                actualPages.Add(page);
        }
        
        if(actualPages.Count % 2 == 0)
            actualPages.Add(emptyPage);
        actualPages.Add(backPage);

        book.bookPages = actualPages.ToArray();

        animator.SetToLastFrame(hide);
    }

    void Update()
    {
        if(isVisible)
        {
            if (Input.GetKeyDown(KeyCode.PageDown))
                autoFlip.FlipLeftPage();

            if (Input.GetKeyDown(KeyCode.PageUp))
                autoFlip.FlipRightPage();

            if (Input.GetKeyDown(KeyCode.Home))
                GotoPage(frontPage, null);

            if (Input.GetKeyDown(KeyCode.End))
                GotoPage(backPage, null);

            if (Input.GetKeyDown(KeyCode.Escape))
                Hide();
        }
        else if (!isVisible)
        {
            if (Input.GetKeyDown(KeyCode.J))
                Show();
        }     
    }

    public static void Show(Page page = null)
    {
        //pause the game

        if(!isVisible)
        {
            isVisible = true;

            //Ensure book starts on front page

            instance.animator.Play(instance.show, () =>
            {
                if (page == null)
                {
                    instance.autoFlip.FlipRightPage();
                }
                else
                {
                    instance.GotoPage(page, null);
                }
            });
        } else
        {
            if (page == null)
            {
                instance.autoFlip.FlipRightPage();
            }
            else
            {
                instance.GotoPage(page, null);
            }
        }                
    }

    public static void Hide()
    {
        instance.GotoPage(instance.frontPage, () =>
        {
            instance.animator.Play(instance.hide, () =>
            {
                isVisible = false;
                //unpause the game
            });
        });        
    }

    void GotoPage(Page page, Action onComplete)
    {
        StopAllCoroutines();
        StartCoroutine(FlipToPage(page, onComplete));
    }

    IEnumerator FlipToPage(Page page, Action onComplete)
    {
        var wait = new WaitForSeconds(autoFlip.PageFlipTime);
        int d = actualPages.IndexOf(page);
        while (book.currentPage > d)
        {
            autoFlip.FlipLeftPage();
            yield return wait;
        }
        while (book.currentPage < d)
        {
            autoFlip.FlipRightPage();
            yield return wait;
        }

        onComplete?.Invoke();
    }
}
