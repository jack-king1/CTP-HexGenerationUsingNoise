using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class PlayButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public TabGroup tabGroup;
    public Image background;
    public UnityEvent onTabSelected, onTabDeselected;

    public bool active = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }

    void Start()
    {
        background = GetComponent<Image>();
        
    }

    public void Selected()
    {
        if(onTabDeselected!= null)
        {   
            onTabSelected.Invoke();
            active = true;
        }
    }

    public void Deselected()
    {
        if (onTabDeselected != null)
        {
            onTabDeselected.Invoke();
            active = false;
        }
    }

    public void MoveUp()
    {
        LeanTween.moveY(gameObject, gameObject.transform.position.y + 400, 0.5f).setEaseInOutBack();
    }

    public void MoveDown()
    {
        LeanTween.moveY(gameObject, gameObject.transform.position.y - 400, 0.5f).setEaseInSine();
    }
}
