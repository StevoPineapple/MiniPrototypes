using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DartBehavior : MonoBehaviour
{
    public float shrinkSpeed = 0.01f;
    private bool shrinking;

    private Vector3 reticlePos;
    float yMax;// max y for ball to travel up to
    float yAccend;
    private Vector3 initPos;

    public float gravity = -0.0022f;

    public float initVSpeed = 0.08f;
    public float initSetVSpeed = 0.035f;
    public float shrinkMinSize = 0.4f;
    public float shrinkAcc = 0.007f;
    public float shrinkAccuracyInfluence = 0.013f;

    float vSpeed;
    float vAcc;
    float hSpeed;

    float accuracy;

    bool hitLock;
    bool hitHuman;

    static GameObject HumanObject;
    BoxCollider2D boxCollider;
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        HumanObject = GameObject.FindGameObjectWithTag("DartsHuman");
        gravity += Random.Range(-0.00015f, 0.00015f)*(1-accuracy); //gravity offset
        initVSpeed += Random.Range(-0.008f, 0.008f) * (1-accuracy); //init vspeed offset
        hSpeed = Random.Range(-0.05f, 0.05f) * (1 - accuracy);

        //vAcc = initVSpeed = 2f;
        vSpeed = initSetVSpeed + initVSpeed*accuracy;
        shrinkAcc += shrinkAccuracyInfluence * accuracy;
        // set max y
        initPos = transform.position;
    }

    void FixedUpdate()
    {
        DartMove();
    }

    void DartMove()
    {
        // shrink
        if (transform.localScale.x > shrinkMinSize)
        {
            shrinkSpeed += shrinkAcc;
            transform.localScale -= new Vector3(shrinkSpeed, shrinkSpeed, 0);
            shrinking = true;
        }
        else
        {
            //boxCollider.enabled = true;
            if (shrinking)
            {
                transform.localScale = new Vector3(shrinkMinSize, shrinkMinSize, 0);
                StartCoroutine(HitCheck());
            }
        }

        //Acc
        vAcc += gravity;
        //move
        vSpeed += vAcc;
        
        if (shrinking)
            transform.position += new Vector3(hSpeed,vSpeed,0);
        else
            transform.position = new Vector3(transform.position.x, transform.position.y, 10f); // Put to back
    }
    public void SetAccuracy(float _accuracy)
    {
        accuracy = _accuracy;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (shrinking)
        {
            if (collision.CompareTag("DartsLock"))
            {
                collision.GetComponent<DartsLockBehaviour>().BreakLock();
            }
            if (collision.CompareTag("DartsHuman"))
            {
                hitHuman = true;
                transform.SetParent(HumanObject.transform, true);
            }
        }
    }
    IEnumerator HitCheck()
    {
        yield return new WaitForFixedUpdate();
        boxCollider.enabled = true;
        yield return new WaitForFixedUpdate();
        if (!hitHuman)
            GetComponent<SpriteRenderer>().sortingOrder = -4;
        boxCollider.enabled = false;
        shrinking = false;
    }
}
