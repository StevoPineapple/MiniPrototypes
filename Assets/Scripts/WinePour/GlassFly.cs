using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassFly : MonoBehaviour
{
    public float rotateSpeed;
    public float moveSpeed;
    Vector3 initPos;
    Quaternion initRotation;
    public bool isRotating;
    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.position;
        initRotation = transform.rotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isRotating)
        {
            transform.Rotate(new Vector3(0,0,rotateSpeed));
            transform.position += new Vector3(moveSpeed, 0.01f, 0);
        }
    }
    public void ResetTrans()
    {
        transform.position = initPos;
        transform.rotation = initRotation;
        isRotating = false;
    }
}
