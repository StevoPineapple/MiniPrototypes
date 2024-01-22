using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoamVolumnBehavior_BB : MonoBehaviour
{
    Vector3 InitScale;

    public SpriteRenderer GreenVolume;
    public SpriteRenderer FoamVolume;
    [SerializeField] private float FoamVolumeTop;
    [SerializeField] private float greenVolumeTop;

    [SerializeField] private float foamVolumeMax;
    [SerializeField] private float foamVolumeMin;

    public float FoamReductionMinRatio;// reduced to how much percent of max foam = 0.7

    public WinePourManager winePourManager; //Legacy
    public BobaPourManager_BB PourManger;

    public AnimationCurve FoamCurve;
    public AnimationCurve FoamRedCurve;
    public AnimationCurve FreshCurve;
    [SerializeField] private float freshness;

    private float _currentVolume;

    [Header ("Debug")]
    public float RDRate;
    public float GRRate;

    void Awake()
    {
        freshness = 1;
        if(InitScale!=null)
            InitScale = transform.localScale;
        else 
            ResetFoam();
    }

    private void FixedUpdate()
    {
        if (PourManger.StopMovement)
        {
            return;
        }
        _currentVolume = transform.localScale.y;
        foamVolumeMin = foamVolumeMax - (foamVolumeMax * FoamReductionMinRatio);
        FoamReduction();
        GrowFoam();

        greenVolumeTop = GreenVolume.bounds.max.y;
        FoamVolumeTop = FoamVolume.bounds.max.y;
        transform.position = new Vector3(transform.position.x, greenVolumeTop, 1);
        transform.localScale = new Vector3(transform.localScale.x, _currentVolume,1);
    }
    public void GrowFoam()
    {
        if (_currentVolume < foamVolumeMax)
        {
            float growScaleRate = ((foamVolumeMax - _currentVolume) / foamVolumeMax); //inverse so closer to 1 = closer to empty
            GRRate = growScaleRate;

            _currentVolume += FoamCurve.Evaluate(growScaleRate) * 0.005f * freshness;
            freshness = Mathf.Clamp(freshness - FreshCurve.Evaluate(freshness/1)*0.003f, 0, 1);//0.005 is est value
        }
    }
    public void GrowFoam(float amount)
    {
        foamVolumeMax += amount;
    }

    void FoamReduction()
    {
        if (_currentVolume > foamVolumeMin)
        {
            float reduceRate = ((_currentVolume - foamVolumeMin) / (foamVolumeMax - foamVolumeMin));
            RDRate = reduceRate;
            //closer to top(more to reduce) = higher(close to 1) 
            //this is the percentage of the current amount of foam vs the max amount of foam
            
            _currentVolume = Mathf.Clamp(_currentVolume - FoamRedCurve.Evaluate(reduceRate)*0.001f,foamVolumeMin,foamVolumeMax);
        }
    }
    public void ResetFoam()
    {
        transform.localScale = InitScale;
        foamVolumeMax = 0;
        freshness = 1;
    }
    public void AddFreshness(float amount)
    {
        freshness = Mathf.Clamp(freshness + amount, 0.01f, 1);
    }
}