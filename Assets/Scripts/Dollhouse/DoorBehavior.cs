using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    bool isHead; //This this door is head
    public Sprite doorCloseSprite;
    public Sprite doorOpenSprite;
    //Animation/opening
    Vector2 initSize;
    Vector2 mouseOverSize;
    bool isOpening;
    Vector2 initPos;

    static bool DoorOpening;

    static DollhouseManager HouseManager;

    void Start()
    {
        if (HouseManager == null)
        {
            HouseManager = GameObject.Find(GameFamilyNames.DollHouseObject).GetComponent<DollhouseManager>();
        }

        initPos = transform.position;
        initSize = transform.localScale;
        mouseOverSize = initSize * 1.1f;
    }

    private void OnMouseExit()
    {
        if (!enabled) return;
        transform.localScale = initSize;
    }
    private void OnMouseOver()
    {
        if (!enabled || isOpening) return; //don't run on disabled or another door opening
        if (!DollhouseManager.HasWon)
        {
            transform.localScale = mouseOverSize;
            if (!DollhouseManager.IsDoorOpening() && Input.GetMouseButtonDown(0)) //check left click
            {
                StartCoroutine(Shaking());
                DollhouseManager.SetDoorCoolDown();
                if (isHead)
                {
                    if (DollhouseManager.IsFirstClick)
                    {
                        HouseManager.ReshufflePosition();
                    }
                    else HouseManager.SetWin();
                }
                DollhouseManager.IsFirstClick = false;
                transform.localScale = initSize;
            }
        }
    }
    public void SetIsHead() //called in Manager
    {
        isHead = true;
    }
    IEnumerator Shaking()
    {
        isOpening = true; //allow shaking
        yield return new WaitForSeconds(DollhouseManager.DoorCoolDownTime);

        GetComponent<SpriteRenderer>().sprite = doorOpenSprite;
        isOpening = false;
        transform.position = initPos;
        enabled = false; //disable self
    }

    public void Update()
    {
        if (isOpening)
            transform.position = new Vector3(initPos.x + Random.Range(-0.05f, 0.05f), initPos.y + Random.Range(-0.05f, 0.05f));
        else
            transform.position = initPos;
    }
    public void resetDoor()
    {
        GetComponent<SpriteRenderer>().sprite = doorCloseSprite;
        isHead = false;
        isOpening = false;//
        //transform.localScale = initSize;
    }
    public void resetDoorSize()
    {
        transform.localScale = initSize;
    }
}
