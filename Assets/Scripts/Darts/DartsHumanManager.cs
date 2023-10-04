using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartsHumanManager : MonoBehaviour
{
    float gravity = -0.005f;
    float yAcc;

    float rotationSpeed;
    float counterClockChance = 0.65f;

    int level; //Difficulty variants

    GameObject[] lockArr;
    //public GameObject splashObj;

    public void Start()
    {
        lockArr = GameObject.FindGameObjectsWithTag("DartsLock");

        rotationSpeed = Random.Range(0.4f, 0.7f); //Rotation Speed
        switch (level)
        {
            case (0):
                {
                    rotationSpeed -= 0.05f;
                    break;
                }
            case(1):
                {
                    rotationSpeed += 0.85f;
                    break;
                }
            case(2):
                {
                    rotationSpeed += 1.7f;
                    break;
                }
            case (3):
                {
                    rotationSpeed = 0;
                    break;
                }
        }
        if (Random.Range(0f, 1f) > counterClockChance)
        {
            //isCounterClock = true;
            rotationSpeed += 0.35f; //CCW is harder but rare
            rotationSpeed = -rotationSpeed;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!CheckAllLocksBreak())
            transform.Rotate(Vector3.forward * rotationSpeed);
        else
        {
            yAcc += gravity;
            transform.position += new Vector3(0, yAcc, 0);
        }
    }
    bool CheckAllLocksBreak()
    {
        foreach (GameObject _lock in lockArr)
        {
            if (_lock.activeSelf)
            {
                return false;
            }
        }
        return true;
    }
    public void SetLevel(int _level)
    {
        level = _level;
    }
}
