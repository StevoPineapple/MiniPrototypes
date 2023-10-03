using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartsHumanManager : MonoBehaviour
{
    float gravity = -0.005f;
    float yAcc;

    float rotationSpeed;
    float counterClockChance = 0.8f;
    //bool hasLock = true;

    GameObject[] lockArr;
    //public GameObject splashObj;

    public void Start()
    {
        lockArr = GameObject.FindGameObjectsWithTag("DartsLock");

        rotationSpeed = Random.Range(0.05f, 0.6f);
        if (Random.Range(0f, 1f) > counterClockChance)
        {
            //isCounterClock = true;
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
}
