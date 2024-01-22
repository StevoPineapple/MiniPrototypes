using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenVolumnBehavior_BB : MonoBehaviour
{
    private Vector3 _InitScale;
    private Vector3 _scale;
    public float GrowLerp; //0.2
    public float GreenVolumeTop;

    public GameObject BobaMask; 

    void Awake()
    {
        if(_InitScale!=null)
            _InitScale = transform.localScale;
        else 
            ResetScale();
        _scale = _InitScale;

        BobaMask.transform.position = transform.position;
    }
    private void FixedUpdate()
    {
        GrowVolume();
        BobaMask.transform.localScale = transform.localScale;
    }
    public void GrowVolume()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _scale, GrowLerp);
    }
    public void GrowVolume(float amount)
    {
            _scale = new Vector3();
            _scale.y += amount;
            _scale += transform.localScale;
    }
    public void ResetScale()
    {
        transform.localScale = _InitScale;
        _scale = _InitScale;
    }
}
