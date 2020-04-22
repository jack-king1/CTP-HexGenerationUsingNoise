using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetColour : MonoBehaviour
{
    public Color clr;
    public Image image;

    public HexMapEditor hme;

    private void Start()
    {
        hme.activeColor = clr;
    }

    public void SetRedColour(float r)
    {
        clr = new Color(r / 255, clr.g, clr.b);
        image.color = clr;
        SetColourInEditor();
    }

    public void SetGreenColour(float g)
    {
        clr = new Color(clr.r, g / 255, clr.b); 
        image.color = clr;
        SetColourInEditor();
    }

    public void SetBlueColour(float b)
    {
        clr = new Color(clr.r, clr.g, b / 255);
        image.color = clr;
        SetColourInEditor();
    }

    void SetColourInEditor()
    {
        hme.activeColor = clr;
    }
}
