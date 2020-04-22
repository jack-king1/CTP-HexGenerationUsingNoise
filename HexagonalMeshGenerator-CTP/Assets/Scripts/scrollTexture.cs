using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrollTexture : MonoBehaviour
{
    public float scrollX;
    public float scrollY;

    public Material texture;

    private void Start()
    {
        texture = GetComponent<Renderer>().material;
    }
    void Update()
    {
        float offsetX = Time.time * scrollX;
        float offsetY = Time.time * scrollY;

        texture.mainTextureOffset = new Vector2(offsetX, offsetY);
    }
}
