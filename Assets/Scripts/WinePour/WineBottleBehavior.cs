using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WineBottleBehavior : MonoBehaviour
{
    float mouseInitX;
    float rotateToMoveRatio = 0.3f;
    float moveLerp = 0.035f;
    float returnLerp = 0.01f;
    bool isDragging;

    Quaternion initRotation;
    float maxRotation = 140f;
    float minRotation;
    float pourThreshold = 90f;

    public Transform bottomRef;
    public Transform topRef;

    LineRenderer lineRend;
    public SpriteRenderer wineSpr;
    public SpriteRenderer wineTopSpr;
    float wineTopSprInitScale;

    float pourYScale;
    float pourAcc;
    float pourYMaxScale = 5f;
    float gravity = 0.003f;

    float pourRadioAdjust = 0.0035f; //fix sprite size scaling to remove this

    public GreenVolumnBehavior_BB wineVolumn;
    public FoamVolumnBehavior_BB foamVolumn;
    public BobaPourManager_BB PourRate;

    Vector3 bottleRotation;
    Coroutine PourRoutine;
    bool CP;
    public float pourIndex;

    // Start is called before the first frame update
    void Start()
    {
        pourIndex = 0.5f;

        initRotation = transform.rotation;
        minRotation = initRotation.z;
        //mouseInitX = Input.mousePosition.x;

        wineSpr.transform.position = bottomRef.position;
        wineTopSpr.transform.position = wineSpr.transform.position;
        wineTopSprInitScale = wineTopSpr.transform.localScale.y;
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (WinePourManager.HasWon || WinePourManager.HasLost)
        {
            DisablePour();
            return;
        }

        bottleRotation = transform.rotation.eulerAngles;
        CheckDragging();
        CheckPouring();
        //DisablePour();
    }
    void StretchWineSprite(float pourRatio)
    {
        if (pourYScale < pourYMaxScale)
        {
            pourAcc += gravity;
            pourYScale += pourAcc;
        }

        //print(wineSpr.size.x);

        Vector3 pourWidthPos = Vector3.Lerp(bottomRef.position, topRef.position, pourRatio);
        //while (wineSpr.bounds.min.x > pourSizeX)
        Vector3 scale = new Vector3((bottomRef.position.x-pourWidthPos.x) / wineSpr.size.x, pourYScale, 0);
        wineSpr.transform.localScale = scale;
        wineSpr.transform.rotation = Quaternion.Euler(0,0,0); // lock rotation
        
        wineTopSpr.transform.rotation = Quaternion.Euler(0,0,0); // lock rotation
        wineTopSpr.transform.localScale = new Vector3(wineSpr.transform.localScale.x, wineTopSprInitScale, wineSpr.transform.localScale.z);
    }

    void CheckDragging()
    {
        if (Input.GetMouseButton(0))
        {
            //Rotation
            if (!isDragging)
            {
                mouseInitX = Input.mousePosition.x;
                isDragging = true;
            }

            bottleRotation.z = -(Input.mousePosition.x - mouseInitX) * rotateToMoveRatio;
            if (bottleRotation.z >= maxRotation)
                bottleRotation.z = maxRotation;
            else if ((bottleRotation.z <= 0))
                bottleRotation.z = 0;
            //bottleRotation.z = Mathf.Lerp(bottleRotation.z, transform.rotation.z, moveLerp);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(bottleRotation), moveLerp);
            //Pour
        }
        else
        {
            isDragging = false;
            transform.rotation = Quaternion.Lerp(transform.rotation, initRotation, returnLerp);
        }
    }
    void CheckPouring()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            PourRoutine = StartCoroutine(DoPour());
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            pourIndex = 0.5f;
            StopCoroutine(PourRoutine);
        }

        if (Input.GetKey(KeyCode.A))
        {
            if (CP)
            {
                pourIndex *= 1.01f;
                wineVolumn.GrowVolume(0.05f*pourIndex); //0.1
                foamVolumn.GrowFoam(0.005f * pourIndex); //0.01
                foamVolumn.AddFreshness(0.025f * pourIndex);//0.05
            }
        }
        else
            CP = false;

        if (transform.rotation.eulerAngles.z > pourThreshold)
        {
            wineTopSpr.enabled = true;
            wineSpr.enabled = true;
            float pourRatio = 1 - ((maxRotation - transform.rotation.eulerAngles.z) / (maxRotation - pourThreshold)); //normalized
            StretchWineSprite(pourRatio * pourRadioAdjust);
            if (wineSpr.bounds.min.y <= wineVolumn.GreenVolumeTop)
            {
                wineVolumn.GrowVolume(pourRatio*0.05f); //0.1
                foamVolumn.GrowFoam(pourRatio*0.005f); //0.01
                foamVolumn.AddFreshness(pourRatio*0.025f);//0.05
            }
        }
        else
        {
            wineTopSpr.enabled = false;
            wineSpr.enabled = false;
            pourAcc = 0;
            pourYScale = 0;
        }
    }
    void DisablePour()
    {
        wineTopSpr.enabled = false;
        wineSpr.enabled = false;
        pourAcc = 0;
        pourYScale = 0;
    }
    IEnumerator DoPour()
    {
        yield return new WaitForSeconds(0.5f);
        CP = true;
    }
}
