using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class DollhouseManager : MiniGameManager
{   
    //called in doors to check cool down state
    static bool IsOpening;
    public static bool HasWon; //used to prevent doors enlarge after win
    
    //For coroutine
    static MonoBehaviour selfInstance;

    //First click prevention
    public static float FirstClickChance = 0.05f;
    public static bool IsFirstClick = true;

    //Doors init
    public static float DoorCoolDownTime = 1.3f;
    static int headDoorPos;
    static int headFloorPos;
    [Serializable] public class Floor
    {
        public DoorBehavior[] doorArr;
        public Sprite[] cthSprArr;
    }
    public Floor[] floorArr;

    //Cthulhu 
    public GameObject headObject;
    public GameObject legObject;

    //public static GameSelect GameSelectManager;
    //public static GameObject WinObject;
    public static void SetDoorCoolDown()
    {
        selfInstance.StartCoroutine(DoorCoolDown());
    }
    static IEnumerator DoorCoolDown()
    {
        IsOpening = true;
        yield return new WaitForSeconds(DoorCoolDownTime);
        IsOpening = false;
    }
    public static bool IsDoorOpening()
    {
        if (IsOpening) return true;
        else return false;
    }
    IEnumerator SetWinIE()
    {
        HasWon = true;
        yield return new WaitForSeconds(DoorCoolDownTime); //wait till door open

        WinObject.SetActive(true);
        //timer
        TimerBehavior.EndTimer();
        GameSelectManager.QuitGame();
    }
    public override void SetWin() //start WIN Coroutine
    {
        selfInstance.StartCoroutine(SetWinIE());
    }
    public override void ResetGame()
    {
        foreach (Floor _floor in floorArr)
        {
            foreach (DoorBehavior _door in _floor.doorArr)
            {
                _door.enabled = true;
                _door.resetDoorSize(); //reset each door size
            }
        }
    }
    public override void StartGame()
    {
        Init();

        IsFirstClick = true;
        float chance = UnityEngine.Random.Range(0f, 1f);
        if (chance <= FirstClickChance)
        { 
            IsFirstClick = false; 
        }

        foreach (Floor _floor in floorArr)
        {
            foreach (DoorBehavior _door in _floor.doorArr)
            {
                _door.resetDoor(); //reset each door
            }
        }

        HasWon = false;
        selfInstance = this;

        ReshufflePosition(); 
        print("cth pattern:" + headFloorPos + "," + headDoorPos);

        //GameSelectManager = GameObject.FindWithTag("GameSelectManager").GetComponent<GameSelect>();
        //WinObject = transform.Find("Win").gameObject;
        //WinObject.GetComponent<SpriteRenderer>().enabled = false;
    }
    public void ReshufflePosition()
    {
        do
        {
            headFloorPos = UnityEngine.Random.Range(0, 3);
            if (headFloorPos == 2)
                headDoorPos = UnityEngine.Random.Range(0, 3);
            else
                headDoorPos = UnityEngine.Random.Range(0, 5);
            
        } while (headDoorPos == 2 && headFloorPos == 1); //Prevent middle spawn

        print("change pattern:" + headFloorPos + "," + headDoorPos);
        SetCthulhuSprite();
    }
    void SetCthulhuSprite()
    {
        floorArr[headFloorPos].doorArr[headDoorPos].SetIsHead();//Set door is head
        legObject.GetComponent<SpriteRenderer>().sprite = floorArr[headFloorPos].cthSprArr[headDoorPos];//set sprite to match door
        headObject.transform.position = floorArr[headFloorPos].doorArr[headDoorPos].transform.position;//set head to door
    }
}
