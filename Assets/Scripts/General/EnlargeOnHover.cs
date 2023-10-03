using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnlargeOnHover : MonoBehaviour
{
    [Header("Size")]
    public bool isCustomSize;
    public float customSize;

    Vector2 targetSizeVec;
    Vector2 initSize;

    [Header("Event")]
    public bool hasEvent;
    public UnityEvent onClickEvent;

    // Start is called before the first frame update
    private void Start()
    {
        initSize = transform.localScale;

        if (!isCustomSize) targetSizeVec = initSize * 1.1f;
        else targetSizeVec = initSize * customSize;
    }
    private void OnMouseOver()
    {
        transform.localScale = targetSizeVec;
        if (hasEvent)
        {
            if (Input.GetMouseButtonDown(0))
            {
                onClickEvent.Invoke();
            }
        }
    }
    private void OnMouseExit()
    {
        transform.localScale = initSize;
    }
}
