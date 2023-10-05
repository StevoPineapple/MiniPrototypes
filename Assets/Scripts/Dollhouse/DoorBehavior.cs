using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DoorBehavior : MonoBehaviour
{
    bool isHead; //This this door is head
    public Sprite doorCloseSprite; //TODO see notes on [SerializeField] private
    public Sprite doorOpenSprite;
    //Animation/opening
    Vector2 initSize;
    Vector2 mouseOverSize;
    bool isOpening;
    Vector2 initPos;

    static bool DoorOpening;

    static DollhouseManager HouseManager;

    [SerializeField] private SpriteRenderer spriteRenderer;

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
        if (!enabled)
        {
            return; //TODO no same line return statements
        }
        transform.localScale = initSize;
    }
    private void OnMouseOver()
    {
        if (!enabled || isOpening)
        {
            return; //don't run on disabled or another door opening
        }

        if (DollhouseManager.HasWon)
        {
            return;
        }
        
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
                else
                {
                    HouseManager.SetWin();
                }
            }
            DollhouseManager.IsFirstClick = false;
            transform.localScale = initSize;
        }
        
    }
    public void SetIsHead() //called in Manager
    {
        isHead = true;
    }
    IEnumerator Shaking() //TODO incorrect Ienumerator naming
    {
        isOpening = true; //allow shaking
        yield return new WaitForSeconds(DollhouseManager.DoorCoolDownTime);

        GetComponent<SpriteRenderer>().sprite = doorOpenSprite; //TODO don't use getcomponent, when you could create a private variable
        isOpening = false;
        transform.position = initPos;
        enabled = false; //disable self
    }

    public void Update()
    {
        if (isOpening) //TODO why is this happening at update?
            transform.position = new Vector3(initPos.x + Random.Range(-0.05f, 0.05f), initPos.y + Random.Range(-0.05f, 0.05f));
        else
            transform.position = initPos;
    }
    public void resetDoor() //TODO incorrect function naming
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
