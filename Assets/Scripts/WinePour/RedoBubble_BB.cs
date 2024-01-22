using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedoBubble_BB : MonoBehaviour
{
    public float LightCycleSpeed = 8f;
    public BobaPourManager_BB PourManager;
    private SpriteRenderer sprRend;
    private Vector3 initSize;

    // Start is called before the first frame update
    void Start()
    {
        sprRend = GetComponent<SpriteRenderer>();
        initSize = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        float h;
        float s;
        float v;
        float sinCycle = (Mathf.Sin(Time.time * LightCycleSpeed) * 0.5f) + 0.5f; // sinwave for y=sin(x*speed)*0.5+0.5
        Color.RGBToHSV(sprRend.color, out h, out s, out v);
        sprRend.color = Color.HSVToRGB(h, s, sinCycle + 0.4f);
    }
    private void OnMouseOver()
    {
        if (!enabled)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            PourManager.RedoPour();
        }
    }
    private void OnMouseEnter()
    {
        if (!enabled)
        {
            return;
        }
        transform.localScale = Vector3.one*1.1f;
    }
    private void OnMouseExit()
    {
        if (!enabled)
        {
            return;
        }
        transform.localScale = Vector3.one;
    }

}
