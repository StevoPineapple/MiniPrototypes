using RitualNight.PartyGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace RitualNight
{
    namespace PartyGames
    {
        public class MoleBehavior_WKM : MonoBehaviour
        {
            public enum MoleType
            {
                Normal,
                Wood,
                Bomb,
                Melon,
                Golden
            }
            [SerializeField] private SpriteRenderer sprRend;
            public MoleType type;
            public bool isHit;
            [HideInInspector] public int HoleIndex;
            [HideInInspector] public MoleSpawnControl_WKM SpawnControl;
            [HideInInspector] public bool IsLastMole;
            [SerializeField] private GameObject particleObj;

            [Tooltip("Time to despawn by itself")]
            [SerializeField] private float goDownTime;
            [SerializeField] private float goDownTimeRange;
            private float _timer;
            private Animator _anim;
            private Collider2D _selfCollider;
            private bool _isDisabled;

            [Header("Golden")]
            [HideInInspector] public int SelfOrder;
            [SerializeField] private TextMeshProUGUI numberText;
            [SerializeField] private PartyGameShaking partyGameShaking;
            private Vector3 _initPos;
            [SerializeField] private Vector3 initPosOffset;

            void Awake()
            {
                _anim = GetComponent<Animator>();
                _selfCollider = GetComponent<Collider2D>();
                _selfCollider.enabled = false;
                _timer = goDownTime + Random.Range(-goDownTimeRange, goDownTimeRange);
            }
            private void Start()
            {
                if (type != MoleType.Bomb)
                {
                    particleObj.SetActive(false);
                }
                if (type == MoleType.Golden)
                {
                    numberText.gameObject.SetActive(true);
                    numberText.text = (SelfOrder+1).ToString();
                    SetInitPos();
                }
                switch (HoleIndex)
                {
                    case 0:
                    case 1:
                    case 2:
                        {
                            sprRend.sortingOrder = -1;
                            break;
                        }
                    case 3:
                    case 4:
                    case 5:
                        {
                            sprRend.sortingOrder = -4;
                            break;
                        }
                    case 6:
                    case 7:
                    case 8:
                        {
                            sprRend.sortingOrder = -7;
                            break;
                        }
                }

            }

            // Update is called once per frame 
            void Update()
            {
                if (SpawnControl.Mode != 1 || type == MoleType.Melon)
                {
                    if (!isHit)
                    {
                        _timer -= Time.deltaTime;
                        if (_timer <= 0)
                        {
                            GoDown();
                        }
                    }
                }
            }
            void SetInitPos() //called in aniamtion
            {
                if (type == MoleType.Golden)
                {
                    partyGameShaking.SetInitPos(transform.position+initPosOffset);
                }
            }
            private void OnTriggerEnter2D(Collider2D collision)
            {
                if (collision.CompareTag("Hammer_WKM"))
                {
                    if (type == MoleType.Golden)
                    {
                        if (SelfOrder != SpawnControl.NextHitGoldenOrder)
                        {
                            partyGameShaking.ShakeStart();
                            return;
                        }
                        else
                        {
                            SpawnControl.NextHitGoldenOrder++;
                        }
                    }
                    GetHit();
                    CheckPoint();
                }
            }
            private void CheckPoint()
            {
                if (type == MoleType.Normal)
                {
                    SpawnControl.WKMManager.AddPoint(1, this);
                }
                else if (type == MoleType.Melon)
                {
                    SpawnControl.WKMManager.AddPoint(2, this);
                }
                else if (type == MoleType.Golden)
                {
                    SpawnControl.WKMManager.AddPoint(1, this);
                }
                if (SpawnControl.WKMManager.HasWon)
                {
                    IsLastMole = true;
                }
            }
            private void GetHit()
            {
                isHit = true;
                _selfCollider.enabled = false;
                _anim.Play("GetHit");
                if (type == MoleType.Golden)
                {
                    numberText.gameObject.SetActive(false);
                }
            }
            public void GoDown()
            {
                _anim.Play("GoDown");
            }
            private void OnDespawn()//Called in animation, use animation to control despawn time
            {
                if (_isDisabled)
                {
                    if(IsLastMole)
                        Destroy(gameObject,0.5f);
                    else
                        Destroy(gameObject);
                    return;
                }
                print(this + HoleIndex.ToString());
                SpawnControl.FreeHole(HoleIndex, this);
                if (type == MoleType.Normal)
                {
                    SpawnControl.ChangeNormalMoleNumber(-1);
                }
                _isDisabled = true;
                if (IsLastMole)
                    Destroy(gameObject, 0.5f);
                else
                    Destroy(gameObject);

            }
            private void ReleaseParticle()//Called in animation
            {
                if (type == MoleType.Bomb)
                {
                    Destroy(Instantiate(particleObj, transform.position, transform.rotation, transform.parent), 2.5f);
                    return;
                }
                particleObj.SetActive(true);
            }
            public void EnableCollider()
            {
                _selfCollider.enabled = true;
            }
            public void DisableCollider()
            {
                _selfCollider.enabled = false;
            }
        }
    }
}