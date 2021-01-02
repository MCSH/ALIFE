using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICell : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;
    public float hue;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.color = Color.HSVToRGB(hue, 1, 1);
    }
}
