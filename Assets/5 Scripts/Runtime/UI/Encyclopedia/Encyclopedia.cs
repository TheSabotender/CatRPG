using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

public class Encyclopedia : MonoBehaviour
{
    private static Encyclopedia instance;

    public Book book;
    public AutoFlip autoFlip;
    public UIAnimator animator;
    public AnimationClip show;
    public AnimationClip hide;

    [Header("Core Pages")]
    public Page frontPage;
    public PageStats indexPage;    
    public PageStats statPage;
    public PageChapter chapterPage;
    public Page emptyPage;
    public Page backPage;

    [Header("Unlocked Pages")]
    public Page skillPage1;
    public Page skillPage2;
    public PageCharacter characterPage1;
    public PageCharacter characterPage2;
    public Page FactionPage1;
    public Page FactionPage2;
    public Page LocationPage;
    public Page EventPage1;
    public Page EventPage2;
    public Page CreaturePage1;
    public Page CreaturePage2;


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

        //Setup
        actualPages = new List<Page>();
        actualPages.Add(AddPage(frontPage));

        //Index
        PageStats index = AddPage(indexPage) as PageStats;
        index.SetData();
        actualPages.Add(index);

        //Stats
        PageStats stats = AddPage(statPage) as PageStats;
        stats.SetData();
        actualPages.Add(stats);

        //SkillTree1
        //SkillTree2

        //Characters
        AddChapter("Characters", null);
        List<CharacterData> characters = GuidDatabase.FindAll<CharacterData>();
        characters.Sort((a, b) => a.order.CompareTo(b.order));
        foreach (var ch in characters)
        {
            if (ch.condition == null || ch.condition.Validate())
            {
                PageCharacter cPage1 = AddPage(characterPage1) as PageCharacter;
                PageCharacter cPage2 = AddPage(characterPage2) as PageCharacter;

                cPage1.SetData(ch);
                cPage2.SetData(ch);

                actualPages.Add(cPage1);
                actualPages.Add(cPage2);
            }
        }

        //Factions
        //Locations
        //Events
        //Creatures        

        //Add end pages
        if (actualPages.Count % 2 == 0)
            actualPages.Add(AddPage(emptyPage));
        actualPages.Add(AddPage(backPage));

        //Apply and hide
        book.bookPages = actualPages.ToArray();        
    }

    private void Start()
    {
        animator.SetToLastFrame(hide);
    }

    void ReloadPages()
    {
        foreach (var page in book.bookPages)
        {
            page.ReloadData();
        }
    }

    Page AddPage(Page page)
    {
        Page p = Instantiate(page, null, false);
        p.gameObject.SetActive(false);
        return p;
    }

    void AddChapter(string header, Sprite icon)
    {
        if (actualPages.Count % 2 == 0)
            actualPages.Add(AddPage(emptyPage));

        actualPages.Add(AddPage(emptyPage));

        PageChapter chapter = AddPage(chapterPage) as PageChapter;
        chapter.SetData(header, icon);
        actualPages.Add(chapter);
    }

    void Update()
    {
        if(isVisible)
        {
            if (Input.GetKeyDown(KeyCode.PageUp))
                autoFlip.FlipLeftPage();

            if (Input.GetKeyDown(KeyCode.PageDown))
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
                Show(1);
        }     
    }

    public static void Show<T>(string search, int indexOffset = 0)
    {
        for (int i = 0; i < instance.actualPages.Count; i++)
        {
            Page page = instance.actualPages[i];
            if (page is T && page.Search(search))
            {
                Show(i + indexOffset);
                return;
            }
        }

        for (int i = 0; i < instance.actualPages.Count; i++)
        {
            Page page = instance.actualPages[i];
            if (page.Search(search))
            {
                Show(i + indexOffset);
                return;
            }
        }
    }

    public static void Show(int index)
    {
        //pause the game

        instance.ReloadPages();

        if (!isVisible)
        {
            isVisible = true;

            //Ensure book starts on front page

            instance.animator.Play(instance.show, () =>
            {
                instance.GotoPage(index, null);
            }, 1, true);
        } else
        {
            instance.GotoPage(index, null);
        }                
    }

    public static void Hide()
    {
        instance.GotoPage(instance.actualPages[0], () =>
        {
            instance.animator.Play(instance.hide, () =>
            {
                isVisible = false;
                //unpause the game
            }, 1, true);
        });        
    }

    void GotoPage(Page page, Action onComplete)
    {
        GotoPage(actualPages.IndexOf(page), onComplete);
    }

    void GotoPage(int page, Action onComplete)
    {
        StopAllCoroutines();
        StartCoroutine(FlipToPage(page, onComplete));
    }

    IEnumerator FlipToPage(int index, Action onComplete)
    {
        var wait = new WaitForSeconds(autoFlip.PageFlipTime);
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
