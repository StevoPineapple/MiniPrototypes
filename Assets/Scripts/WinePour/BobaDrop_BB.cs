using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobaDrop_BB : MonoBehaviour
{
    public bool IsStopped;
    public bool IsShrinking;
    public float Gravity;// = 0.02f;
    public float InitAcc;// = -0.03f;
    float _gravAcc;

    [HideInInspector] public BobaPourManager_BB PourRate;
    [HideInInspector] public PourButton_BB PourButton;

    // Start is called before the first frame update
    private void Start()
    {
        _gravAcc = InitAcc;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (IsStopped)
        {
            return;
        }
        if (IsShrinking)
        {
            transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
            PourButton.PourIncomplete(transform.localScale.x);
            if (transform.localScale.x <= 0)
            {
                IsStopped = true;
            }
            return;
        }
        transform.position -= new Vector3(0, _gravAcc, 0);
        _gravAcc += Gravity;
        if (transform.position.y <= PourRate.FoamTopPosition)
        {
            IsShrinking = true;
        }
    }
    public void SetInitialTransform(float size, Vector3 positionChange)
    {
        transform.localScale = new Vector3(size, size, 1);
        transform.position += positionChange;
        InitAcc = Random.Range(-0.03f, 0.01f);
}
}
