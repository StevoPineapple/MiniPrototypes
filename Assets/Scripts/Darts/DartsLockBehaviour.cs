using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartsLockBehaviour : MonoBehaviour
{
    float lightCycleSpeed = 4f;
    SpriteRenderer sprRend;
    public GameObject debrisObject;
    // Start is called before the first frame update
    void Start()
    {
        sprRend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float h;
        float s;
        float v;
        float sinCycle = (Mathf.Sin(Time.time * lightCycleSpeed)*0.5f)+0.5f; // sinwave for y=sin(x*speed)*0.5+0.5
        Color.RGBToHSV(sprRend.color, out h, out s, out v);
        sprRend.color = Color.HSVToRGB(h, s, sinCycle+0.2f);
            
    }
    /*IEnumerator Breaking()
    {
        Instantiate(debrisObject, transform.position, transform.rotation);
        gameObject.SetActive(false);
        yield return new WaitForSeconds(0);
    }*/
    public void BreakLock()
    {
        GameObject debris;
        for(int i = 0;i<3;i++)
        {
            debris = Instantiate(debrisObject, transform.position, transform.rotation);
            debris.transform.SetParent(DartsManager.DebrisHolder.transform);
        }
        
        gameObject.SetActive(false);
    }
}
