using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartsReticle : MonoBehaviour
{
    public GameObject outerRing;
    public GameObject outerRingHold;

    float ringMinSize = 0.25f;//TODO Expose this variables in inspector
    float ringShrinkSpeed = 0.023f*60;
    float ringInitSize; //Assume X and Y size is the same
    float ringInitAlpha = 0.3f;

    float dartCoolDown = 0.3f;
    bool canThrow = true;

    float accuracy;

    public GameObject dartObj;
    public GameObject dartsFamily;

    public GameObject debugPosition;
    // Start is called before the first frame update
    void Start()
    {
        ringInitSize = outerRing.transform.localScale.x;
    }
    // Update is called once per frame //Todo this function has a lot going on, separating this in own functions
    void Update()
    {
        //follow mouse
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(worldPos.x,worldPos.y,-1f);
        //transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        if (canThrow)
        {
            if (Input.GetMouseButton(0))
            {
                if (outerRing.transform.localScale.x > ringMinSize)
                {
                    Color ringColor = outerRing.GetComponent<SpriteRenderer>().color;

                    float initToCurrent = ringInitSize - outerRing.transform.localScale.x;
                    float initToMin = ringInitSize - ringMinSize;

                    accuracy = initToCurrent / initToMin;

                    outerRing.GetComponent<SpriteRenderer>().color = new Color(ringColor.r, ringColor.g, ringColor.b, ((0.8f * accuracy + 0.3f)));
                    outerRingHold.GetComponent<SpriteRenderer>().color = new Color(ringColor.r, ringColor.g, ringColor.b, (1 - accuracy*2.5f));

                    outerRing.transform.localScale = new Vector2
                      (outerRing.transform.localScale.x - ringShrinkSpeed * Time.deltaTime, outerRing.transform.localScale.y - ringShrinkSpeed * Time.deltaTime);
                }
                else
                    outerRing.transform.localScale = new Vector2(ringMinSize, ringMinSize);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Color ringColor = outerRing.GetComponent<SpriteRenderer>().color; //TODO avoid get component if possible
                outerRing.GetComponent<SpriteRenderer>().color = new Color(ringColor.r, ringColor.g, ringColor.b, ringInitAlpha);
                outerRingHold.GetComponent<SpriteRenderer>().color = new Color(ringColor.r, ringColor.g, ringColor.b, 1);

                float initToCurrent = ringInitSize - outerRing.transform.localScale.x;
                float initToMin = ringInitSize - ringMinSize;

                accuracy = (initToCurrent / initToMin);
                //print("IC" + IC + "IM" + IM);
                outerRing.transform.localScale = new Vector2(ringInitSize, ringInitSize);
                ShootDart(accuracy);
            }
        }
    }
    void ShootDart(float accuracy)
    {
        DartBehavior dart = Instantiate(dartObj,transform.position,transform.rotation).GetComponent<DartBehavior>();

        dart.transform.SetParent(DartsManager.DartsHolder.transform);
        dart.SetAccuracy(accuracy);

        StartCoroutine(DartCoolDown());
        canThrow = false;
    }

    IEnumerator DartCoolDown()
    {
        yield return new WaitForSeconds(dartCoolDown);
        canThrow = true;
    }

}
