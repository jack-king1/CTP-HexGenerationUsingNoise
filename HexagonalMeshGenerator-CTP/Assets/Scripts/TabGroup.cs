using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtonsList;
    public Sprite tabIdle, tabHover, tabActive;
    public TabButton selectedTab;
    public List<GameObject> objectsToSwap;

    public void Subscribe(TabButton button)
    {
        if(tabButtonsList == null)
        {
            tabButtonsList = new List<TabButton>();
        }

        tabButtonsList.Add(button);
    }

    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
        if (selectedTab == null || button != selectedTab)
        {
            button.background.sprite = tabHover;
        }
        
    }

    public void OnTabExit(TabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton button)
    {
        if(selectedTab != null && selectedTab != button)
        {
            selectedTab.Deselected();
        }
        if(selectedTab == button && button.active)
        {
            button.Deselected();
            selectedTab = null;
        }
        else
        {
            selectedTab = button;
            selectedTab.Selected();
            ResetTabs();
            button.background.sprite = tabActive;
            int index = button.transform.GetSiblingIndex();
            for (int i = 0; i < objectsToSwap.Count; ++i)
            {
                if (i == index)
                {
                    objectsToSwap[i].SetActive(true);
                }
                else
                {
                    objectsToSwap[i].SetActive(false);
                }
            }
        }
    }

    public void ResetTabs()
    {
        foreach(TabButton btn in tabButtonsList)
        {
            if(selectedTab!= null && btn == selectedTab)
            {
                continue;
            }
            btn.background.sprite = tabIdle;
        }
    }
}
