using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WineVolumnBehavior : MonoBehaviour
{
    SpriteRenderer sprRend;
    Vector3 InitScale;
    float maxVolumn = 0.91f; //measured in editor
    int winStayTickMax = 50;
    int winTick;
    float growLerp = 0.8f;
    
    bool isGrowing;

    public float wineVolumeTop;

    public WinePourManager winePourManager;

    void Start()
    {
        winTick = winStayTickMax;
        if(InitScale!=null)
            InitScale = transform.localScale;
        else 
            ResetScale();
        sprRend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!WinePourManager.HasWon||!WinePourManager.HasLost) wineVolumeTop = sprRend.bounds.max.y;

    }
    private void FixedUpdate()
    {
        if (!WinePourManager.HasWon&&!WinePourManager.HasLost)
        {
            SpriteRenderer targetSprRend = WinePourManager.TargetRange.GetComponent<SpriteRenderer>();
            if (wineVolumeTop <= targetSprRend.bounds.max.y && wineVolumeTop >= targetSprRend.bounds.min.y)
            {
                if (!isGrowing)
                {
                    if (winTick > 0)
                        winTick--;
                    else if(!WinePourManager.HasWon)
                        winePourManager.SetWin();
                }
                else
                    winTick = winStayTickMax;
            }
            else if (wineVolumeTop!=0&& wineVolumeTop > targetSprRend.bounds.max.y)
            {
                winePourManager.SetAngry();
            }
            isGrowing = false;
        }
    }
    public void GrowVolume(float amount)
    {
        if (transform.localScale.y < maxVolumn)
        {
            Vector3 scale = new Vector3();
            scale.y += amount;
            scale += transform.localScale;
            transform.localScale = Vector3.Lerp(scale,transform.localScale,growLerp);
        }
        else transform.localScale = new Vector3(transform.localScale.x, maxVolumn, transform.localScale.z);
        isGrowing = true;
    }
    public void ResetScale()
    {
        transform.localScale = InitScale;
    }
}
