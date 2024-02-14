using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveMenu : MonoBehaviour
{
    public Book book;
    public AutoFlip autoFlip;
    public UIAnimator animator;
    public AnimationClip show;
    public AnimationClip hide;

    [Header("Pages")]
    public Page frontPage;
    public SaveMenu_List listPage;
    public SaveMenu_Data dataPage;

    private List<Page> actualPages;
    private bool isVisible;
    private bool isSaving;

    private void Awake()
    {
        //Setup
        actualPages = new List<Page>();

        //Pages
        frontPage.gameObject.SetActive(false);
        actualPages.Add(frontPage);

        listPage.gameObject.SetActive(false);
        actualPages.Add(listPage);

        dataPage.gameObject.SetActive(false);
        actualPages.Add(dataPage);

        //Apply and hide
        book.bookPages = actualPages.ToArray();
    }

    private void Start()
    {
        animator.SetToLastFrame(hide);
    }
    
    void Update()
    {
        if (isVisible)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Hide();
        }
    }

    public void Show(bool isSaving)
    {
        this.isSaving = isSaving;

        listPage.Setup(this, isSaving);
        dataPage.Setup(null);

        if (!isVisible)
        {
            isVisible = true;

            animator.Play(show, () =>
            {
                GotoPage(1, null);
            }, 1, true);
        }
    }

    public void Hide()
    {        
        GotoPage(0, () =>
        {
            animator.Play(hide, () =>
            {
                isVisible = false;
            }, 1, true);
        });
    }

    void GotoPage(int page, Action onComplete)
    {
        StopAllCoroutines();
        StartCoroutine(FlipToPage(page, onComplete));
    }

    IEnumerator FlipToPage(int index, Action onComplete)
    {
        var wait = new WaitForSecondsRealtime(autoFlip.PageFlipTime);
        while (book.currentPage > index)
        {
            autoFlip.FlipLeftPage();
            yield return wait;
        }
        while (book.currentPage < index)
        {
            autoFlip.FlipRightPage();
            yield return wait;
        }

        onComplete?.Invoke();
    }
}
