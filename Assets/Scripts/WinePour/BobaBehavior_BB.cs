using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobaBehavior_BB : MonoBehaviour
{
    public float MinSpeed;
    public float MaxSpeed;
    public float MinScale;
    public float MaxScale;
    private float _speed;
    public float amplitude;
    private float _initialYPos;
    [HideInInspector] public SpriteRenderer SprRend;
    public SpriteRenderer EdgeSprRend;

    void Awake()
    {
        SprRend = GetComponent<SpriteRenderer>();

        float scale = Random.Range(MinScale, MaxScale);
        transform.localScale = new Vector3(scale, scale, 1);
        _speed = Random.Range(MinSpeed, MaxSpeed);
        _initialYPos = transform.position.y;
        transform.position = new Vector3(transform.position.x, _initialYPos, transform.position.z);
    }

    void Update()
    {
        float yPos = Mathf.Sin(Time.time * _speed) * amplitude;
        transform.position = new Vector3(transform.position.x, yPos + _initialYPos +amplitude , transform.position.z);
    }
    public void UpdateAmplitude(float amp)
    {
        amplitude = amp;
    }
}



