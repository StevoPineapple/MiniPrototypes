using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartsDebris : MonoBehaviour
{
    float gravity = -0.01f;
    float breakSpeedRange = 0.2f;
    float initBreakSpeed;
    float initBreakVSpeedRange = 0.1f;
    float initBreakVSpeed;
    Vector3 velocity;

    float rotationSpeed;
    float counterClockChance = 0.5f;
    bool isCounterClock;
    // Start is called before the first frame update
    void Start()
    {
        initBreakSpeed = Random.Range(-breakSpeedRange, breakSpeedRange);
        initBreakVSpeed = Random.Range(-initBreakVSpeedRange, initBreakVSpeedRange);
        if (initBreakSpeed < 0)
            initBreakSpeed -= 0.02f;
        else
            initBreakSpeed += 0.02f;

        velocity = new Vector3(initBreakSpeed,initBreakVSpeed,0);

        rotationSpeed = Random.Range(115f, 114f);
        if (Random.Range(0f, 1f) > counterClockChance)
        {
            isCounterClock = true;
            rotationSpeed = -rotationSpeed;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        velocity = new Vector3(initBreakSpeed, velocity.y + gravity,0);
        transform.position += velocity;
        transform.Rotate(Vector3.forward * rotationSpeed);
    }
}
