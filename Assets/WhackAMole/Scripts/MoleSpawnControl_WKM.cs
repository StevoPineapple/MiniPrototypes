using RitualNight.PartyGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RitualNight
{
    namespace PartyGames
    {
        public class MoleSpawnControl_WKM : MonoBehaviour
        {
            // -----
            // 6 7 8
            // 3 4 5 
            // 0 1 2
            [HideInInspector] public int Mode;
            [HideInInspector] public int NextHitGoldenOrder;
            [SerializeField] private List<int> emptyHoleList;
            [SerializeField] private List<MoleBehavior_WKM> moleList;
            [SerializeField] private Transform[] holeTransArr;
            [SerializeField] private Transform moleParent;

            private int _normalMoleNumber;

            [Header("Tuning")]
            [Tooltip("if is in chance, pick again between bomb and melon")]
            [SerializeField] private float bombChance;
            [SerializeField] private float melonChance;
            [SerializeField] private float mode0SpawnTime;
            [SerializeField] private float mode0SpawnTimeRange;
            [SerializeField] private float mode1SpawnTime;
            [SerializeField] private float mode1SpawnTimeRange;

            [Header("MolePrefab")]
            [SerializeField] private GameObject normalMole;
            [SerializeField] private GameObject woodMole;
            [SerializeField] private GameObject molen;
            [SerializeField] private GameObject bomb;
            [SerializeField] private GameObject goldenMole;

            [Header("Manager")]
            public WhackAMoleTaskBehavior WKMManager;
            // Start is called before the first frame update
            public void StartSpawning(int mode)
            {
                emptyHoleList = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
                moleList = new List<MoleBehavior_WKM>();
                _normalMoleNumber = 0;
                Mode = mode;
                StartCoroutine(DoSpawning(mode));
            }
            IEnumerator DoSpawning(int mode)
            {
                yield return new WaitForSeconds(0.2f);
                if (mode == 0)
                {
                    while (!WKMManager.HasWon)
                    {
                        Mode0InitSpawn();
                        yield return new WaitForSeconds(mode0SpawnTime + Random.Range(-mode0SpawnTimeRange, mode0SpawnTimeRange));
                    }
                }
                else if (mode == 1)
                {
                    AllMolesGoDown();
                    Mode1Spawn();
                    yield return null;//new WaitForSeconds(mode1SpawnTime + Random.Range(-mode1SpawnTimeRange, mode1SpawnTimeRange));
                }
                else if (mode == 2)
                {
                    Mode2Spawn();//SpawnGoldenMoles();
                }
            }
            void SpawnMole(int num)
            {
                if (WKMManager.HasWon)
                {
                    return;
                }
                for (int i = 0; i < num; i++)
                {
                    if (emptyHoleList.Count < 1)
                    {
                        print("no empty hole");
                        return;
                    }
                    int _chooseIndex = Random.Range(0, emptyHoleList.Count);
                    MoleBehavior_WKM _mole = Instantiate(normalMole, holeTransArr[emptyHoleList[_chooseIndex]].position, transform.rotation, moleParent).GetComponent<MoleBehavior_WKM>();
                    _mole.SpawnControl = this;
                    _mole.HoleIndex = emptyHoleList[_chooseIndex];
                    moleList.Add(_mole);
                    emptyHoleList.Remove(emptyHoleList[_chooseIndex]);
                    ChangeNormalMoleNumber(1);
                }
            }

            void SpawnNotMole(int num)
            {
                if (WKMManager.HasWon)
                {
                    return;
                }
                for (int i = 0; i < num; i++)
                {
                    if (emptyHoleList.Count < 1)
                    {
                        print("no empty hole");
                        return;
                    }
                    GameObject obj = woodMole;
                    if (Random.Range(0f, 1f) < bombChance)
                    {
                        obj = bomb;
                        if (Random.Range(0f, 1f) < melonChance)
                        {
                            obj = molen;
                        }
                    }
                    int _chooseIndex = Random.Range(0, emptyHoleList.Count);
                    MoleBehavior_WKM _mole = Instantiate(obj, holeTransArr[emptyHoleList[_chooseIndex]].position, transform.rotation, moleParent).GetComponent<MoleBehavior_WKM>();
                    _mole.SpawnControl = this;
                    _mole.HoleIndex = emptyHoleList[_chooseIndex];
                    moleList.Add(_mole);
                    emptyHoleList.Remove(emptyHoleList[_chooseIndex]);
                }
            }
            void SpawnGoldenMoles()
            {
                for (int i = 0; i < 9; i++)
                {
                    int _chooseIndex = Random.Range(0, emptyHoleList.Count);
                    MoleBehavior_WKM _mole = Instantiate(goldenMole, holeTransArr[emptyHoleList[_chooseIndex]].position, transform.rotation, moleParent).GetComponent<MoleBehavior_WKM>();
                    _mole.SpawnControl = this;
                    _mole.HoleIndex = emptyHoleList[_chooseIndex];
                    _mole.SelfOrder = i;
                    moleList.Add(_mole);
                    emptyHoleList.Remove(emptyHoleList[_chooseIndex]);
                }
            }
            public void FreeHole(int index, MoleBehavior_WKM mole)
            {
                moleList.Remove(mole);
                emptyHoleList.Add(index);
            }

            public void ChangeNormalMoleNumber(int change)
            {
                _normalMoleNumber += change;
                if (Mode == 0)
                {
                    if (_normalMoleNumber == 0)
                    {
                        Mode0Spawn();
                    }
                }
                else if (Mode == 1)
                {
                    if (_normalMoleNumber == 0)
                    {
                        AllMolesGoDown();
                        Mode1Spawn();
                    }
                }
            }
            public void AllMolesGoDown()
            {
                foreach (MoleBehavior_WKM mole in moleList)
                {
                    mole.GoDown();
                }
            }
            public void AllMolesGoDown(MoleBehavior_WKM exceptionMole)
            {
                foreach (MoleBehavior_WKM mole in moleList)
                {
                    if (mole != exceptionMole)
                    {
                        mole.GoDown();
                    }
                }
            }
            public void ClearMoleParent()
            {
                foreach (Transform t in moleParent)
                {
                    Destroy(t.gameObject);
                }
            }
            public void StopCoroutine()
            {
                StopAllCoroutines();
            }
            void Mode0InitSpawn()
            {
                SpawnNotMole(1);
                SpawnMole(1);
                SpawnNotMole(1);
            }

            void Mode0Spawn()
            {
                SpawnNotMole(1);
                SpawnMole(1);
                SpawnNotMole(1);
            }

            void Mode1Spawn()
            {
                if (!WKMManager.HasWon)
                {
                    StartCoroutine(DoMode1Spawn());
                }
            }
            void Mode2Spawn()
            {
                NextHitGoldenOrder = 0;
                SpawnGoldenMoles();
            }
            IEnumerator DoMode1Spawn()
            {
                while (moleList.Count != 0)
                {
                    yield return new WaitForEndOfFrame();
                }
                SpawnMole(1);
                SpawnNotMole(8);
            }
        }
    }
}