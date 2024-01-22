using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobaCup_BB : MonoBehaviour
{
    public bool HaveSleeve;
    public GameObject SleeveObj;
    public AnimationCurve CupCurve;
    public SpriteRenderer TargetRangeSprite;
    public Transform BobaHolder;
    private List<BobaBehavior_BB> BobaList = new List<BobaBehavior_BB>();

    private void Start()
    {
        int depthCount = 0;
        foreach (Transform _bobaTrans in BobaHolder)
        {
            BobaBehavior_BB _boba = _bobaTrans.GetComponent<BobaBehavior_BB>();            
            _boba.SprRend.sortingOrder = depthCount;
            depthCount++;
            _boba.EdgeSprRend.sortingOrder = depthCount;
            depthCount++;
            BobaList.Add(_boba);
        }
    }
    public void UpdateBobaAmplitude(float amp)
    {
        foreach (BobaBehavior_BB _boba in BobaList)
        {
            _boba.UpdateAmplitude(amp);
        }
    }
    public void SortBoba()
    {
        
    }
}
