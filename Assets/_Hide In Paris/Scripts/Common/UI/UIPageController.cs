using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIPageController : MonoBehaviour
{
    [SerializeField] private UIPage startPage;

    [SerializeField] private List<UIPage> pages = new List<UIPage>();

    private UIPage currentPage;
    private UIPage previousPage;

    private void Start() => ShowPage(startPage);

    public void ShowPage(UIPage page)
    {
        HideAllPages();
        GetPage(page).Show();
        previousPage = currentPage;
        currentPage = page;
    }

    private void HideAllPages()
    {
        foreach (UIPage page in pages)
        {
            page.Hide();
        }
    }
    public void ShowGamePage() => ShowPage(startPage);
    public void BackToPreviousPage() => ShowPage(previousPage);
    private UIPage GetPage(UIPage pageToShow) => pages.First(page => page == pageToShow);

}
